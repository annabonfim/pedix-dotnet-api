using Xunit;

using System.Net;

using System.Net.Http.Headers;

using System.Net.Http.Json;

using System.Text;

using System.Text.Json;

namespace Atendimentos.IntegrationTests
{
    public class ClienteIntegrationTests
        : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        // =====================================================
        // 🏗️ CONSTRUTOR
        // =====================================================
        public ClienteIntegrationTests(
            CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        // =====================================================
        // 🔐 GERA TOKEN JWT
        // =====================================================
        private async Task<string> ObterTokenAsync()
        {
            // ==========================================
            // 📧 EMAIL ÚNICO
            // ==========================================
            var email =
                $"{Guid.NewGuid()}@email.com";

            // ==========================================
            // 👤 REGISTRO
            // ==========================================
            var registerJson =
                JsonSerializer.Serialize(new
                {
                    nome = "Usuario Teste",

                    email = email,

                    senha = "123456",

                    telefone = "11999999999",

                    dataNascimento = "2000-05-10"
                });

            await _client.PostAsync(
                "/api/auth/register-cliente",

                new StringContent(
                    registerJson,
                    Encoding.UTF8,
                    "application/json"));

            // ==========================================
            // 🔐 LOGIN
            // ==========================================
            var loginJson =
                JsonSerializer.Serialize(new
                {
                    email = email,

                    senha = "123456"
                });

            var loginResponse =
                await _client.PostAsync(
                    "/api/auth/login",

                    new StringContent(
                        loginJson,
                        Encoding.UTF8,
                        "application/json"));

            // ==========================================
            // 📦 RESPONSE
            // ==========================================
            var content =
                await loginResponse.Content
                    .ReadAsStringAsync();

            // ==========================================
            // ❌ VALIDA LOGIN
            // ==========================================
            if (!loginResponse.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Erro login: {content}");
            }

            // ==========================================
            // 🎫 TOKEN
            // ==========================================
            using var doc =
                JsonDocument.Parse(content);

            var token =
                doc.RootElement
                   .GetProperty("token")
                   .GetString();

            return token!;
        }

        // =====================================================
        // ➕ POST CLIENTE
        // =====================================================
        [Fact]
        public async Task PostCliente_DeveCriarCliente()
        {
            // ==========================================
            // ARRANGE
            // ==========================================

            // 🔐 TOKEN
            var token =
                await ObterTokenAsync();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            // 👤 NOVO CLIENTE
            var novoCliente =
                new
                {
                    nome = "Maria",

                    telefone = "11999999999"
                };

            // ==========================================
            // ACT
            // ==========================================
            var response =
                await _client.PostAsJsonAsync(
                    "/api/clientes",
                    novoCliente);

            // ==========================================
            // ASSERT
            // ==========================================
            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);
        }

        // =====================================================
        // 📋 GET CLIENTES
        // =====================================================
        [Fact]
        public async Task GetClientes_DeveRetornarSucesso()
        {
            // ==========================================
            // ARRANGE
            // ==========================================

            // 🔐 TOKEN
            var token =
                await ObterTokenAsync();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            // ==========================================
            // ACT
            // ==========================================
            var response =
                await _client.GetAsync(
                    "/api/clientes");

            // ==========================================
            // ASSERT
            // ==========================================
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        // =====================================================
        // ❌ LOGIN INVÁLIDO
        // =====================================================
        [Fact]
        public async Task Login_SenhaInvalida_DeveRetornarBadRequest()
        {
            // ==========================================
            // ARRANGE
            // ==========================================

            var email =
                $"{Guid.NewGuid()}@email.com";

            // 👤 REGISTRA USUÁRIO
            var registerJson =
                JsonSerializer.Serialize(new
                {
                    nome = "Maria",

                    email = email,

                    senha = "123456",

                    telefone = "11999999999",

                    dataNascimento = "2000-05-10"
                });

            await _client.PostAsync(
                "/api/auth/register-cliente",

                new StringContent(
                    registerJson,
                    Encoding.UTF8,
                    "application/json"));

            // 🔐 LOGIN COM SENHA ERRADA
            var loginJson =
                JsonSerializer.Serialize(new
                {
                    email = email,

                    senha = "senha_errada"
                });

            // ==========================================
            // ACT
            // ==========================================
            var response =
                await _client.PostAsync(
                    "/api/auth/login",

                    new StringContent(
                        loginJson,
                        Encoding.UTF8,
                        "application/json"));

            // ==========================================
            // ASSERT
            // ==========================================
            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);
        }
    }
}