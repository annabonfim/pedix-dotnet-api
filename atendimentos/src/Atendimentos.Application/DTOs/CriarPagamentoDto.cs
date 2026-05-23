namespace Atendimentos.Application.DTOs
{
    // Body do POST /api/pagamentos
    public class CriarPagamentoDto
    {
        public Guid PedidoId { get; set; }

        public decimal Valor { get; set; }

        // Aceita: PIX, CREDITO, DEBITO, DINHEIRO
        public string MetodoPagamento { get; set; } = string.Empty;
    }
}
