using Microsoft.EntityFrameworkCore;

using Atendimentos.Domain.Entities;

using Atendimentos.Domain.Repositories;

using Atendimentos.Infrastructure.Context;

namespace Atendimentos.Infrastructure.Repositories
{
    public class PedidoRepository
        : IPedidoRepository
    {
        private readonly AtendimentosDbContext _context;

        // =====================================================
        // 🏗️ CONSTRUTOR
        // =====================================================
        public PedidoRepository(
            AtendimentosDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // ✅ CRIAR PEDIDO
        // =====================================================
        public async Task<Pedido> CriarAsync(
            Pedido pedido)
        {
            await _context.Pedidos
                .AddAsync(pedido);

            await _context.SaveChangesAsync();

            return pedido;
        }

        // =====================================================
        // 📋 LISTAR PEDIDOS
        // =====================================================
        public async Task<List<Pedido>>
            ObterTodosAsync()
        {
            return await _context.Pedidos
                .ToListAsync();
        }

        // =====================================================
        // 🔍 BUSCAR POR ID
        // =====================================================
        public async Task<Pedido?>
            ObterPorIdAsync(Guid id)
        {
            return await _context.Pedidos
                .FirstOrDefaultAsync(
                    p => p.Id == id);
        }

        // =====================================================
        // 🔄 ATUALIZAR PEDIDO (status, valor)
        // =====================================================
        public async Task AtualizarAsync(
            Pedido pedido)
        {
            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }

        // =====================================================
        // 🔍 LISTAR POR CLIENTE
        // =====================================================
        public async Task<List<Pedido>>
            ObterPorClienteAsync(Guid clienteId)
        {
            return await _context.Pedidos
                .Where(p => p.ClienteId == clienteId)
                .ToListAsync();
        }

        // =====================================================
        // 🔍 LISTAR POR MESA
        // =====================================================
        public async Task<List<Pedido>>
            ObterPorMesaAsync(Guid mesaId)
        {
            return await _context.Pedidos
                .Where(p => p.MesaId == mesaId)
                .ToListAsync();
        }

        // =====================================================
        // 🔍 LISTAR POR GARÇOM
        // =====================================================
        public async Task<List<Pedido>>
            ObterPorGarcomAsync(Guid garcomId)
        {
            return await _context.Pedidos
                .Where(p => p.GarcomId == garcomId)
                .ToListAsync();
        }
    }
}
