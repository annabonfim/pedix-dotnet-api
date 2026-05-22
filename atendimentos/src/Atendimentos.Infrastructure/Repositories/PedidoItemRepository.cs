using Microsoft.EntityFrameworkCore;

using Atendimentos.Domain.Entities;

using Atendimentos.Domain.Repositories;

using Atendimentos.Infrastructure.Context;

namespace Atendimentos.Infrastructure.Repositories
{
    public class PedidoItemRepository
        : IPedidoItemRepository
    {
        private readonly AtendimentosDbContext _context;

        public PedidoItemRepository(
            AtendimentosDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // ✅ CRIAR ITEM
        // =====================================================
        public async Task<PedidoItem>
            CriarAsync(PedidoItem item)
        {
            await _context.PedidoItens
                .AddAsync(item);

            await _context.SaveChangesAsync();

            return item;
        }

        // =====================================================
        // 📋 LISTAR TODOS
        // =====================================================
        public async Task<List<PedidoItem>>
            ObterTodosAsync()
        {
            return await _context.PedidoItens
                .ToListAsync();
        }

        // =====================================================
        // 🔍 LISTAR POR PEDIDO
        // =====================================================
        public async Task<List<PedidoItem>>
            ObterPorPedidoAsync(Guid pedidoId)
        {
            return await _context.PedidoItens
                .Where(p => p.PedidoId == pedidoId)
                .ToListAsync();
        }
    }
}