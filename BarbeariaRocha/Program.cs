using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Aplicacao.Servicos;
using BarbeariaRocha.Configurations;
using BarbeariaRocha.Infraestrutura;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Infraestrutura.Middlewares;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------- DATABASE --------------------

builder.Services.AddDbContext<Contexto>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// -------------------- CONTROLLERS --------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// -------------------- OPENAPI (.NET 10 Nativo) --------------------

builder.Services.AddSwaggerConfiguration();

// -------------------- DEPENDENCY INJECTION --------------------

builder.Services.AddScoped<IAgendamentoApp, AgendamentoApp>();
builder.Services.AddScoped<IAutenticacaoApp, AutenticacaoApp>();
builder.Services.AddScoped<IUsuarioApp, UsuarioApp>();
builder.Services.AddScoped<IServicoApp, ServicoApp>();
builder.Services.AddScoped<ITokenApp, TokenApp>();
builder.Services.AddScoped<IExcecaoApp, ExcecaoApp>();
builder.Services.AddScoped<IMensalistaApp, MensalistaApp>();
builder.Services.AddScoped<ITestesApp, TestesApp>();
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

// -------------------- PIPELINE --------------------

app.UseSwaggerConfiguration();

app.UseHttpsRedirection();

app.UseCors("MinhaPoliticaCors");

app.UseAuthentication();   // ⚠ IMPORTANTE (faltava no seu código)
app.UseAuthorization();

app.MapControllers();

app.Run();