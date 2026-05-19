using Atendimentos.Application.DTOs.Auth;

using Atendimentos.Domain.Entities;

namespace Atendimentos.Application.Services.Auth
{
    public interface IAuthService
    {
        // =====================================================
        // 👤 REGISTRO CLIENTE
        // =====================================================
        Task<Usuario> RegistrarClienteAsync(
            RegisterClienteDto dto);

        // =====================================================
        // 🧑‍🍳 REGISTRO GARÇOM
        // =====================================================
        Task<Usuario> RegistrarGarcomAsync(
            RegisterGarcomDto dto);

        // =====================================================
        // 👑 REGISTRO ADMIN
        // =====================================================
        Task<Usuario> RegistrarAdminAsync(
            RegisterAdminDto dto);

        // =====================================================
        // 🔐 LOGIN JWT
        // =====================================================
        Task<string> LoginAsync(
            LoginDto dto);
    }
}