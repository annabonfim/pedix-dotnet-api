using Atendimentos.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

using Atendimentos.Domain.Repositories;

using Atendimentos.Infrastructure.Repositories;

using Atendimentos.Application.Services;
using Atendimentos.Application.Services.Auth;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using System.Text.Json;
using System.Linq;

using Serilog;

using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// 🧾 CONFIGURAÇÃO DO SERILOG
// ==========================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ==========================
// 🔌 BANCO DE DADOS (ORACLE)
// ==========================
builder.Services.AddDbContext<AtendimentosDbContext>(options =>
    options.UseOracle(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================
// 📊 HEALTH CHECKS
// ==========================
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<AtendimentosDbContext>("Database");

// ==========================
// 🔍 OPENTELEMETRY
// ==========================
builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService("Atendimentos.Api"))

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

// ==========================
// 📦 INJEÇÃO DE DEPENDÊNCIA
// ==========================

// 🪑 MESA
builder.Services.AddScoped<IMesaRepository, MesaRepository>();
builder.Services.AddScoped<IMesaService, MesaService>();

// 🧑‍🍳 GARÇOM
builder.Services.AddScoped<IGarcomRepository, GarcomRepository>();
builder.Services.AddScoped<IGarcomService, GarcomService>();

// 🧾 COMANDA
builder.Services.AddScoped<IComandaRepository, ComandaRepository>();
builder.Services.AddScoped<IComandaService, ComandaService>();

// 👤 CLIENTE
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();

// 🔐 USUÁRIO / AUTH
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();

// ==========================
// ⚙️ CONFIGURAÇÕES GERAIS
// ==========================
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// ==========================
// 🚀 BUILD APP
// ==========================
var app = builder.Build();

// ==========================
// 📘 SWAGGER
// ==========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

// ==========================
// 🧾 LOG DE REQUISIÇÕES
// ==========================
app.UseSerilogRequestLogging();

// ==========================
// 🔐 MIDDLEWARES
// ==========================
app.UseHttpsRedirection();

app.UseAuthorization();

// ==========================
// 📍 CONTROLLERS
// ==========================
app.MapControllers();

// ==========================
// ❤️ HEALTH CHECK CUSTOMIZADO
// ==========================
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),

            totalDuration = report.TotalDuration,

            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,

                status = entry.Value.Status.ToString(),

                duration = entry.Value.Duration
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

// ==========================
// ▶️ START APP
// ==========================
app.Run();

// ==========================
// 🧪 TESTES DE INTEGRAÇÃO
// ==========================
public partial class Program
{
}