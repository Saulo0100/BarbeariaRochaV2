using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Aplicacao.Servicos;
using BarbeariaRocha.Configurations;
using BarbeariaRocha.Infraestrutura;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Infraestrutura.Middlewares;
using BarbeariaRocha.Infraestrutura.MultiTenancy;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// -------------------- MULTI-TENANCY --------------------

builder.Services.AddHttpContextAccessor();
builder.Services.Configure<TenantOptions>(
    builder.Configuration.GetSection(TenantOptions.SectionName)
);
builder.Services.AddScoped<ITenantService, TenantService>();

// -------------------- TENANT VALIDATION (API EXTERNA) --------------------

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ITenantValidationService, TenantValidationService>();
builder.Services.AddHttpClient("TenantValidation", client =>
{
    var baseUrl = builder.Configuration["Apis:ConfiguracaoBaseUrl"]
        ?? throw new InvalidOperationException("Apis:ConfiguracaoBaseUrl não está configurado.");
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(2);
});

// -------------------- DATABASE --------------------

// Banco de dados único — tenant_id nas tabelas identifica cada estabelecimento.
builder.Services.AddDbContext<Contexto>((serviceProvider, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// -------------------- CONTROLLERS --------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// -------------------- OPENAPI (.NET 10 Nativo) --------------------

builder.Services.AddSwaggerConfiguration();

// -------------------- DEPENDENCY INJECTION --------------------

builder.Services.AddScoped<IAgendamentoApp, AgendamentoApp>();
builder.Services.AddScoped<IAutenticacaoApp, AutenticacaoApp>();
builder.Services.AddScoped<IUsuarioApp, UsuarioApp>();
builder.Services.AddScoped<IEmailApp, EmailApp>();
builder.Services.AddScoped<IServicoApp, ServicoApp>();
builder.Services.AddScoped<ITokenApp, TokenApp>();
builder.Services.AddScoped<IExcecaoApp, ExcecaoApp>();
builder.Services.AddScoped<IMensalistaApp, MensalistaApp>();
builder.Services.AddScoped<ITestesApp, TestesApp>();
builder.Services.AddScoped<IRelatorioApp, RelatorioApp>();
builder.Services.AddScoped<IHorarioApp, HorarioApp>();
builder.Services.AddScoped<IConfiguracaoHorarioApp, ConfiguracaoHorarioApp>();
builder.Services.AddScoped<IAdicionalApp, AdicionalApp>();
builder.Services.AddScoped<IConfiguracaoBarbeariaApp, ConfiguracaoBarbeariaApp>();
builder.Services.AddSingleton<TokenProvider>();

// -------------------- JWT --------------------

var jwtSecret = builder.Configuration["Jwt:Secret"]
        ?? throw new InvalidOperationException("Jwt:Secret não está configurado.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)
            ),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// -------------------- CORS --------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("MinhaPoliticaCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// -------------------- HANGFIRE --------------------

builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("DefaultConnection")
        );
    })
    .UseSimpleAssemblyNameTypeSerializer();
});

builder.Services.AddHangfireServer();

// -------------------- BUILD --------------------

var app = builder.Build();

// -------------------- MIDDLEWARE --------------------

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TenantValidationMiddleware>();
app.UseMiddleware<TenantMiddleware>();

// -------------------- PIPELINE --------------------

app.UseSwaggerConfiguration();

app.UseHttpsRedirection();

app.UseCors("MinhaPoliticaCors");

app.UseAuthentication();   // ⚠ IMPORTANTE (faltava no seu código)
app.UseAuthorization();

app.MapControllers();

app.Run();
