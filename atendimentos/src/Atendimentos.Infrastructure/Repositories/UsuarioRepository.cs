using Atendimentos.Domain.Entities;
using Atendimentos.Domain.Repositories;
using Atendimentos.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Atendimentos.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AtendimentosDbContext _context;

        public UsuarioRepository(AtendimentosDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario> CriarAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);

            await _context.SaveChangesAsync();

            return usuario;
        }
    }
}