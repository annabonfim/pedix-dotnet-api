using Atendimentos.Domain.Entities;

namespace Atendimentos.Domain.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmailAsync(string email);

        Task<Usuario> CriarAsync(Usuario usuario);
    }
}