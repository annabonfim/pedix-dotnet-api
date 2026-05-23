using Atendimentos.Domain.Entities;

using Atendimentos.Domain.Repositories;

namespace Atendimentos.Application.Services
{
    public class PagamentoService
        : IPagamentoService
    {
        private readonly IPagamentoRepository _repository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IPedidoService _pedidoService;

        // Métodos de pagamento aceitos
        private static readonly HashSet<string> _metodosValidos =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "PIX",
                "CREDITO",
                "DEBITO",
                "DINHEIRO",
            };

        // Status que já são terminais — não viram ENTREGUE de novo.
        private static readonly HashSet<string> _statusTerminais =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "ENTREGUE",
                "CANCELADO",
            };

        // =====================================================
        // 🏗️ CONSTRUTOR
        // =====================================================
        public PagamentoService(
            IPagamentoRepository repository,
            IPedidoRepository pedidoRepository,
            IPedidoService pedidoService)
        {
            _repository = repository;
            _pedidoRepository = pedidoRepository;
            _pedidoService = pedidoService;
        }

        // =====================================================
        // ✅ CRIAR PAGAMENTO
        // =====================================================
        public async Task<Pagamento>
            CriarAsync(
                Guid pedidoId,
                decimal valor,
                string metodoPagamento)
        {
            if (pedidoId == Guid.Empty)
            {
                throw new ArgumentException(
                    "PedidoId é obrigatório.");
            }

            if (valor <= 0)
            {
                throw new ArgumentException(
                    "Valor do pagamento deve ser maior que zero.");
            }

            if (string.IsNullOrWhiteSpace(metodoPagamento)
                || !_metodosValidos.Contains(metodoPagamento))
            {
                throw new ArgumentException(
                    $"Método de pagamento inválido. " +
                    $"Válidos: {string.Join(", ", _metodosValidos)}.");
            }

            var pagamento =
                new Pagamento(
                    pedidoId,
                    valor,
                    metodoPagamento.ToUpperInvariant());

            return await _repository
                .CriarAsync(pagamento);
        }

        // =====================================================
        // 🔍 BUSCAR POR ID
        // =====================================================
        public async Task<Pagamento?>
            ObterPorIdAsync(Guid id)
        {
            return await _repository
                .ObterPorIdAsync(id);
        }

        // =====================================================
        // 🔍 LISTAR POR PEDIDO
        // =====================================================
        public async Task<List<Pagamento>>
            ObterPorPedidoAsync(Guid pedidoId)
        {
            return await _repository
                .ObterPorPedidoAsync(pedidoId);
        }

        // =====================================================
        // 📋 LISTAR TODOS
        // =====================================================
        public async Task<List<Pagamento>>
            ObterTodosAsync()
        {
            return await _repository
                .ObterTodosAsync();
        }

        // =====================================================
        // ✅ APROVAR PAGAMENTO
        // =====================================================
        // Quando o pagamento é aprovado, fecha a conta do cliente naquela mesa:
        // todos os pedidos ativos do mesmo cliente+mesa viram ENTREGUE. Isso
        // dispara a liberação da mesa em cascata via PedidoService.
        public async Task<Pagamento?>
            AprovarAsync(Guid id)
        {
            var pagamento = await _repository
                .ObterPorIdAsync(id);

            if (pagamento == null) return null;

            pagamento.Aprovar();

            await _repository.AtualizarAsync(pagamento);

            await FecharContaAsync(pagamento.PedidoId);

            return pagamento;
        }

        // =====================================================
        // 🧾 FECHAR CONTA (helper)
        // =====================================================
        private async Task FecharContaAsync(Guid pedidoIdPago)
        {
            var pedidoBase = await _pedidoRepository
                .ObterPorIdAsync(pedidoIdPago);

            if (pedidoBase == null) return;

            var pedidosDoCliente = await _pedidoRepository
                .ObterPorClienteAsync(pedidoBase.ClienteId);

            var pedidosParaFechar = pedidosDoCliente
                .Where(p => p.MesaId == pedidoBase.MesaId)
                .Where(p => !_statusTerminais.Contains(p.Status));

            foreach (var pedido in pedidosParaFechar)
            {
                await _pedidoService
                    .AtualizarStatusAsync(pedido.Id, "ENTREGUE");
            }
        }

        // =====================================================
        // ❌ RECUSAR PAGAMENTO
        // =====================================================
        public async Task<Pagamento?>
            RecusarAsync(Guid id)
        {
            var pagamento = await _repository
                .ObterPorIdAsync(id);

            if (pagamento == null) return null;

            pagamento.Recusar();

            await _repository.AtualizarAsync(pagamento);

            return pagamento;
        }
    }
}
