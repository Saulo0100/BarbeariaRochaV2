using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Aplicacao.Servicos;
using BarbeariaRocha.Configurations;
using BarbeariaRocha.Infraestrutura;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Infraestrutura.Middlewares;
using BarbeariaRocha.Infraestrutura.MultiTenancy;
using BarbeariaRocha.Infraestrutura.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;

// -------------------- SERILOG --------------------

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// -------------------- MULTI-TENANCY --------------------

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantService, TenantService>();

// -------------------- TENANT VALIDATION --------------------

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ITenantValidationService, TenantValidationService>();

// -------------------- DATABASE --------------------

builder.Services.AddDbContext<Contexto>((serviceProvider, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// -------------------- HEALTH CHECKS --------------------

builder.Services.AddHealthChecks()
    .AddCheck("api", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

// -------------------- CONTROLLERS --------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// -------------------- OPENAPI --------------------

if (!builder.Environment.IsProduction())
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
builder.Services.AddScoped<ITenantAdminApp, TenantAdminApp>();
builder.Services.AddScoped<IWhatsappService, WhatsappService>();
builder.Services.AddSingleton<TokenProvider>();

// -------------------- REPOSITORY (generic open-generic) --------------------

builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

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

var corsOrigins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>()
    ?? ["https://localhost:44396"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("MinhaPoliticaCors", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// -------------------- BUILD --------------------

var app = builder.Build();

// -------------------- MIDDLEWARE --------------------

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TenantValidationMiddleware>();

// -------------------- PIPELINE --------------------

if (!builder.Environment.IsProduction())
    app.UseSwaggerConfiguration();

app.UseHttpsRedirection();

app.UseCors("MinhaPoliticaCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
