namespace Atendimentos.Domain.Entities
{
    public class PedidoItem
    {
        public Guid Id { get; private set; }

        public Guid PedidoId { get; private set; }

        public string NomeProduto { get; private set; }

        public int Quantidade { get; private set; }

        public decimal PrecoUnitario { get; private set; }

        public decimal Subtotal { get; private set; }

        // ==========================================
        // 🏗️ CONSTRUTOR
        // ==========================================
        public PedidoItem(
            Guid pedidoId,
            string nomeProduto,
            int quantidade,
            decimal precoUnitario)
        {
            Id = Guid.NewGuid();

            PedidoId = pedidoId;

            NomeProduto = nomeProduto;

            Quantidade = quantidade;

            PrecoUnitario = precoUnitario;

            Subtotal =
                quantidade * precoUnitario;
        }

        protected PedidoItem()
        {
        }
    }
}