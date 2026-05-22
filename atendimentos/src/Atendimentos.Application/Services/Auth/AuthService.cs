using Atendimentos.Application.DTOs.Auth;

using Atendimentos.Domain.Entities;

using Atendimentos.Domain.Repositories;

using Microsoft.Extensions.Configuration;

using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;

using System.Text;

namespace Atendimentos.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _repository;

        private readonly IConfiguration _configuration;

        // =====================================================
        // 🏗️ CONSTRUTOR
        // =====================================================
        public AuthService(
            IUsuarioRepository repository,
            IConfiguration configuration)
        {
            _repository = repository;

            _configuration = configuration;
        }

        // =====================================================
        // 👤 REGISTRO CLIENTE
        // =====================================================
        public async Task<Usuario> RegistrarClienteAsync(
            RegisterClienteDto dto)
        {
            var usuarioExistente =
                await _repository
                    .ObterPorEmailAsync(dto.Email);

            if (usuarioExistente != null)
            {
                throw new Exception(
                    "Email já cadastrado.");
            }

            var senhaHash =
                BCrypt.Net.BCrypt
                    .HashPassword(dto.Senha);

            var usuario =
                new Usuario(
                    dto.Nome,
                    dto.Email,
                    senhaHash,
                    dto.Telefone,
                    dto.DataNascimento,
                    "Cliente");

            return await _repository
                .CriarAsync(usuario);
        }

        // =====================================================
        // 👨‍🍳 REGISTRO GARÇOM
        // =====================================================
        public async Task<Usuario> RegistrarGarcomAsync(
            RegisterGarcomDto dto)
        {
            var usuarioExistente =
                await _repository
                    .ObterPorEmailAsync(dto.Email);

            if (usuarioExistente != null)
            {
                throw new Exception(
                    "Email já cadastrado.");
            }

            var senhaHash =
                BCrypt.Net.BCrypt
                    .HashPassword(dto.Senha);

            var usuario =
                new Usuario(
                    dto.Nome,
                    dto.Email,
                    senhaHash,
                    dto.Telefone,
                    dto.DataNascimento,
                    "Garcom",
                    dto.CPF,
                    dto.Matricula);

            return await _repository
                .CriarAsync(usuario);
        }

        // =====================================================
        // 👑 REGISTRO ADMIN
        // =====================================================
        public async Task<Usuario> RegistrarAdminAsync(
            RegisterAdminDto dto)
        {
            var adminKeyConfig =
                _configuration[
                    "AdminSettings:AdminKey"];

            if (dto.AdminKey != adminKeyConfig)
            {
                throw new Exception(
                    "AdminKey inválida.");
            }

            var usuarioExistente =
                await _repository
                    .ObterPorEmailAsync(dto.Email);

            if (usuarioExistente != null)
            {
                throw new Exception(
                    "Email já cadastrado.");
            }

            var senhaHash =
                BCrypt.Net.BCrypt
                    .HashPassword(dto.Senha);

            var usuario =
                new Usuario(
                    dto.Nome,
                    dto.Email,
                    senhaHash,
                    dto.Telefone,
                    dto.DataNascimento,
                    "Admin",
                    null,
                    null,
                    dto.AdminKey);

            return await _repository
                .CriarAsync(usuario);
        }

        // =====================================================
        // 🔐 LOGIN JWT
        // =====================================================
        public async Task<string> LoginAsync(
            LoginDto dto)
        {
            var usuario =
                await _repository
                    .ObterPorEmailAsync(dto.Email);

            if (usuario == null)
            {
                throw new Exception(
                    "Email ou senha inválidos.");
            }

            var senhaValida =
                BCrypt.Net.BCrypt.Verify(
                    dto.Senha,
                    usuario.SenhaHash);

            if (!senhaValida)
            {
                throw new Exception(
                    "Email ou senha inválidos.");
            }

            // =================================================
            // 📌 CLAIMS JWT
            // =================================================
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    usuario.Nome),

                new Claim(
                    ClaimTypes.Email,
                    usuario.Email),

                new Claim(
                    ClaimTypes.Role,
                    usuario.Role)
            };

            // =================================================
            // 🔑 JWT KEY
            // =================================================
            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _configuration["Jwt:Key"]!));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            // =================================================
            // 🎫 TOKEN
            // =================================================
            var token =
                new JwtSecurityToken(
                    issuer:
                        _configuration["Jwt:Issuer"],

                    audience:
                        _configuration["Jwt:Audience"],

                    claims:
                        claims,

                    expires:
                        DateTime.UtcNow.AddHours(2),

                    signingCredentials:
                        credentials);

            // =================================================
            // 🚀 RETURN TOKEN
            // =================================================
            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}