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

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.IdentityModel.Tokens;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// 🧾 SERILOG
// =====================================================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/log-.txt",
        rollingInterval:
            RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// =====================================================
// 🔌 DATABASE ORACLE
// =====================================================
builder.Services.AddDbContext<
    AtendimentosDbContext>(options =>
{
    options.UseOracle(
        builder.Configuration
            .GetConnectionString(
                "DefaultConnection"));
});

// =====================================================
// ❤️ HEALTH CHECKS
// =====================================================
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<
        AtendimentosDbContext>(
            "Database");

// =====================================================
// 🔍 OPENTELEMETRY
// =====================================================
builder.Services
    .AddOpenTelemetry()

    .ConfigureResource(resource =>
        resource.AddService(
            "Atendimentos.Api"))

    // =================================================
    // 🔎 TRACING
    // =================================================
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()

            .AddHttpClientInstrumentation()

            .AddConsoleExporter();
    })

    // =================================================
    // 📈 METRICS
    // =================================================
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
    builder.Configuration["Jwt:Key"]

    ?? throw new Exception(
        "JWT Key não configurada.");

var key =
    Encoding.UTF8.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)

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
                    new SymmetricSecurityKey(key)
            };
    });

// =====================================================
// 📦 DEPENDENCY INJECTION
// =====================================================

// =====================================================
// 🍽️ MESA
// =====================================================
builder.Services.AddScoped<
    IMesaRepository,
    MesaRepository>();

builder.Services.AddScoped<
    IMesaService,
    MesaService>();

// =====================================================
// 👨‍🍳 GARÇOM
// =====================================================
builder.Services.AddScoped<
    IGarcomRepository,
    GarcomRepository>();

builder.Services.AddScoped<
    IGarcomService,
    GarcomService>();

// =====================================================
// 🧾 COMANDA
// =====================================================
builder.Services.AddScoped<
    IComandaRepository,
    ComandaRepository>();

builder.Services.AddScoped<
    IComandaService,
    ComandaService>();

// =====================================================
// 👤 CLIENTE
// =====================================================
builder.Services.AddScoped<
    IClienteRepository,
    ClienteRepository>();

builder.Services.AddScoped<
    IClienteService,
    ClienteService>();

// =====================================================
// 🔐 USUÁRIO / AUTH
// =====================================================
builder.Services.AddScoped<
    IUsuarioRepository,
    UsuarioRepository>();

builder.Services.AddScoped<
    IAuthService,
    AuthService>();

// =====================================================
// 🧾 PEDIDOS
// =====================================================
builder.Services.AddScoped<
    IPedidoRepository,
    PedidoRepository>();

builder.Services.AddScoped<
    IPedidoService,
    PedidoService>();

// =====================================================
// 📦 PEDIDO ITENS
// =====================================================
builder.Services.AddScoped<
    IPedidoItemRepository,
    PedidoItemRepository>();

builder.Services.AddScoped<
    IPedidoItemService,
    PedidoItemService>();

// =====================================================
// ⚙️ CONTROLLERS
// =====================================================
builder.Services.AddControllers();

// =====================================================
// 📘 SWAGGER
// =====================================================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new()
        {
            Title = "Atendimentos API",

            Version = "v1"
        });

    // ================================================
    // 🔐 JWT SWAGGER
    // ================================================
    options.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type =
                Microsoft.OpenApi.Models
                    .SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT",

            In =
                Microsoft.OpenApi.Models
                    .ParameterLocation.Header,

            Description =
                "Digite o token JWT."
        });

    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models
            .OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models
                    .OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models
                            .OpenApiReference
                        {
                            Type =
                                Microsoft.OpenApi.Models
                                    .ReferenceType
                                        .SecurityScheme,

                            Id = "Bearer"
                        }
                },

                Array.Empty<string>()
            }
        });
});

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
// 🧾 REQUEST LOGGING
// =====================================================
app.UseSerilogRequestLogging();

// =====================================================
// 🔐 MIDDLEWARES
// =====================================================
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// =====================================================
// 📍 CONTROLLERS
// =====================================================
app.MapControllers();

// =====================================================
// ❤️ HEALTH CHECK
// =====================================================
app.MapHealthChecks(
    "/health",
    new HealthCheckOptions
    {
        ResponseWriter =
            async (context, report) =>
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
                        report.Entries.Select(
                            entry => new
                            {
                                name = entry.Key,

                                status =
                                    entry.Value.Status
                                        .ToString(),

                                duration =
                                    entry.Value.Duration
                            })
                };

                await context.Response
                    .WriteAsync(
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
// 🧪 TESTES INTEGRAÇÃO
// =====================================================
public partial class Program
{
}