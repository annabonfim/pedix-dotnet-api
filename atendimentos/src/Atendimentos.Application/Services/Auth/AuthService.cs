using Atendimentos.Application.DTOs.Auth;
using Atendimentos.Domain.Entities;
using Atendimentos.Domain.Repositories;

namespace Atendimentos.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<Usuario> RegistrarClienteAsync(RegisterClienteDto dto);
    }

    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _repository;

        public AuthService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<Usuario> RegistrarClienteAsync(RegisterClienteDto dto)
        {
            var usuarioExistente =
                await _repository.ObterPorEmailAsync(dto.Email);

            if (usuarioExistente != null)
            {
                throw new Exception("Email já cadastrado.");
            }

            var senhaHash =
                BCrypt.Net.BCrypt.HashPassword(dto.Senha);

            var usuario = new Usuario(
                dto.Nome,
                dto.Email,
                senhaHash,
                dto.Telefone,
                dto.DataNascimento,
                "Cliente");

            return await _repository.CriarAsync(usuario);
        }
    }
}