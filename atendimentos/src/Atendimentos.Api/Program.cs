using Atendimentos.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;

using Atendimentos.Domain.Repositories;

using Atendimentos.Infrastructure.Repositories;

using Atendimentos.Application.Services;
using Atendimentos.Application.Services.Auth;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.IdentityModel.Tokens;

using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

using Serilog;

using System.Text;
using System.Text.Json;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// 🧾 CONFIGURAÇÃO DO SERILOG
// =====================================================

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()

    .WriteTo.Console()

    .WriteTo.File(
        "logs/log-.txt",
        rollingInterval: RollingInterval.Day)

    .CreateLogger();

builder.Host.UseSerilog();

// =====================================================
// 🔌 BANCO DE DADOS ORACLE
// =====================================================

builder.Services.AddDbContext<AtendimentosDbContext>(options =>
{
    options.UseOracle(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"));
});

// =====================================================
// 📊 HEALTH CHECKS
// =====================================================

builder.Services
    .AddHealthChecks()

    .AddDbContextCheck<AtendimentosDbContext>(
        "Database");

// =====================================================
// 🔍 OPENTELEMETRY
// =====================================================

builder.Services
    .AddOpenTelemetry()

    .ConfigureResource(resource =>
    {
        resource.AddService("Atendimentos.Api");
    })

    // 🔎 TRACING
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()

            .AddHttpClientInstrumentation()

            .AddConsoleExporter();
    })

    // 📈 MÉTRICAS
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()

            .AddHttpClientInstrumentation()

            .AddConsoleExporter();
    });

// =====================================================
// 🔐 JWT AUTHENTICATION
// =====================================================

var jwtKey =
    builder.Configuration["Jwt:Key"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })

    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
            };
    });

// =====================================================
// 📦 INJEÇÃO DE DEPENDÊNCIA
// =====================================================

// 🪑 MESA
builder.Services.AddScoped<
    IMesaRepository,
    MesaRepository>();

builder.Services.AddScoped<
    IMesaService,
    MesaService>();

// 🧑‍🍳 GARÇOM
builder.Services.AddScoped<
    IGarcomRepository,
    GarcomRepository>();

builder.Services.AddScoped<
    IGarcomService,
    GarcomService>();

// 🧾 COMANDA
builder.Services.AddScoped<
    IComandaRepository,
    ComandaRepository>();

builder.Services.AddScoped<
    IComandaService,
    ComandaService>();

// 👤 CLIENTE
builder.Services.AddScoped<
    IClienteRepository,
    ClienteRepository>();

builder.Services.AddScoped<
    IClienteService,
    ClienteService>();

// 🔐 AUTH / USUÁRIOS
builder.Services.AddScoped<
    IUsuarioRepository,
    UsuarioRepository>();

builder.Services.AddScoped<
    IAuthService,
    AuthService>();

// =====================================================
// ⚙️ CONFIGURAÇÕES GERAIS
// =====================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// =====================================================
// 🚀 BUILD APP
// =====================================================

var app = builder.Build();

// =====================================================
// 📘 SWAGGER
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

// =====================================================
// 🧾 LOGS DE REQUISIÇÃO
// =====================================================

app.UseSerilogRequestLogging();

// =====================================================
// 🔐 MIDDLEWARES
// =====================================================

app.UseHttpsRedirection();

// 🔐 JWT
app.UseAuthentication();

app.UseAuthorization();

// =====================================================
// 📍 CONTROLLERS
// =====================================================

app.MapControllers();

// =====================================================
// ❤️ HEALTH CHECK CUSTOMIZADO
// =====================================================

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType =
            "application/json";

        var response = new
        {
            status =
                report.Status.ToString(),

            totalDuration =
                report.TotalDuration,

            checks =
                report.Entries.Select(entry => new
                {
                    name = entry.Key,

                    status =
                        entry.Value.Status.ToString(),

                    duration =
                        entry.Value.Duration
                })
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
    }
});

// =====================================================
// ▶️ START APP
// =====================================================

app.Run();

// =====================================================
// 🧪 TESTES DE INTEGRAÇÃO
// =====================================================

public partial class Program
{
}