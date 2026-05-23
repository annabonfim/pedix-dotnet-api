using Microsoft.EntityFrameworkCore;

using Atendimentos.Domain.Entities;

using Atendimentos.Domain.Repositories;

using Atendimentos.Infrastructure.Context;

namespace Atendimentos.Infrastructure.Repositories
{
    public class PagamentoRepository
        : IPagamentoRepository
    {
        private readonly AtendimentosDbContext _context;

        // =====================================================
        // 🏗️ CONSTRUTOR
        // =====================================================
        public PagamentoRepository(
            AtendimentosDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // ✅ CRIAR PAGAMENTO
        // =====================================================
        public async Task<Pagamento> CriarAsync(
            Pagamento pagamento)
        {
            await _context.Pagamentos
                .AddAsync(pagamento);

            await _context.SaveChangesAsync();

            return pagamento;
        }

        // =====================================================
        // 🔍 BUSCAR POR ID
        // =====================================================
        public async Task<Pagamento?>
            ObterPorIdAsync(Guid id)
        {
            return await _context.Pagamentos
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // =====================================================
        // 🔍 LISTAR POR PEDIDO
        // =====================================================
        public async Task<List<Pagamento>>
            ObterPorPedidoAsync(Guid pedidoId)
        {
            return await _context.Pagamentos
                .Where(p => p.PedidoId == pedidoId)
                .ToListAsync();
        }

        // =====================================================
        // 📋 LISTAR TODOS
        // =====================================================
        public async Task<List<Pagamento>>
            ObterTodosAsync()
        {
            return await _context.Pagamentos
                .ToListAsync();
        }

        // =====================================================
        // 🔄 ATUALIZAR PAGAMENTO
        // =====================================================
        public async Task AtualizarAsync(
            Pagamento pagamento)
        {
            _context.Pagamentos.Update(pagamento);
            await _context.SaveChangesAsync();
        }
    }
}
