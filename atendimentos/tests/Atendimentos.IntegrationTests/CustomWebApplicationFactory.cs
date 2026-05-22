using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Mvc.Testing;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Authentication;

using Microsoft.Extensions.Options;

using System.Security.Claims;

using System.Text.Encodings.Web;

using Atendimentos.Infrastructure.Context;

namespace Atendimentos.IntegrationTests
{
    public class CustomWebApplicationFactory
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            // =====================================================
            // ⚙️ CONFIG TEST JWT
            // =====================================================
            builder.ConfigureAppConfiguration(
                (context, config) =>
                {
                    config.AddInMemoryCollection(
                        new Dictionary<string, string?>
                        {
                            {
                                "Jwt:Key",
                                "TEST_SECRET_KEY_123456789_ULTRA_SAFE"
                            },

                            {
                                "Jwt:Issuer",
                                "Atendimentos.Api"
                            },

                            {
                                "Jwt:Audience",
                                "Atendimentos.Client"
                            },

                            {
                                "AdminSettings:AdminKey",
                                "FIAP_ADMIN_MASTER_2026"
                            }
                        });
                });

            // =====================================================
            // 🔧 CONFIG SERVICES
            // =====================================================
            builder.ConfigureServices(services =>
            {
                // ==========================================
                // ❌ REMOVE ORACLE
                // ==========================================
                var descriptor =
                    services.SingleOrDefault(
                        d =>
                            d.ServiceType ==
                            typeof(
                                DbContextOptions<
                                    AtendimentosDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // ==========================================
                // ✅ DATABASE IN MEMORY
                // ==========================================
                services.AddDbContext<
                    AtendimentosDbContext>(
                    options =>
                    {
                        options.UseInMemoryDatabase(
                            "IntegrationTestsDb");
                    });

                // ==========================================
                // 🔐 AUTH TEST
                // ==========================================
                services
                    .AddAuthentication("Test")
                    .AddScheme<
                        AuthenticationSchemeOptions,
                        TestAuthHandler>(
                            "Test",
                            options => { });

                // ==========================================
                // 🔄 BUILD SERVICES
                // ==========================================
                var serviceProvider =
                    services.BuildServiceProvider();

                using var scope =
                    serviceProvider.CreateScope();

                var db =
                    scope.ServiceProvider
                        .GetRequiredService<
                            AtendimentosDbContext>();

                // ==========================================
                // 🧹 RESET DATABASE
                // ==========================================
                db.Database.EnsureDeleted();

                db.Database.EnsureCreated();
            });
        }
    }

    // =====================================================
    // 🔐 TEST AUTH HANDLER
    // =====================================================
    public class TestAuthHandler
        : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult>
            HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.Name,
                    "UsuarioTeste"),

                new Claim(
                    ClaimTypes.Email,
                    "teste@email.com"),

                new Claim(
                    ClaimTypes.Role,
                    "Admin")
            };

            var identity =
                new ClaimsIdentity(
                    claims,
                    "Test");

            var principal =
                new ClaimsPrincipal(identity);

            var ticket =
                new AuthenticationTicket(
                    principal,
                    "Test");

            var result =
                AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}