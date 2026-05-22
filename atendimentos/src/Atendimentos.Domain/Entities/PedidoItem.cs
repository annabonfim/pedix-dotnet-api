namespace Atendimentos.Domain.Entities
{
    public class PedidoItem
    {
        public Guid Id { get; private set; }

        public Guid PedidoId { get; private set; }

        // =====================================================
        // 🍔 ITEM CARDÁPIO (API JAVA)
        // =====================================================
        public int ItemCardapioId { get; private set; }

        // =====================================================
        // 🔢 QUANTIDADE
        // =====================================================
        public int Quantidade { get; private set; }

        // =====================================================
        // 💰 PREÇO NO MOMENTO DA COMPRA
        // =====================================================
        public decimal PrecoMomento { get; private set; }

        // =====================================================
        // 💵 SUBTOTAL
        // =====================================================
        public decimal Subtotal { get; private set; }

        // =====================================================
        // 🏗️ CONSTRUTOR
        // =====================================================
        public PedidoItem(
            Guid pedidoId,
            int itemCardapioId,
            int quantidade,
            decimal precoMomento)
        {
            Id = Guid.NewGuid();

            PedidoId = pedidoId;

            ItemCardapioId = itemCardapioId;

            Quantidade = quantidade;

            PrecoMomento = precoMomento;

            Subtotal =
                quantidade * precoMomento;
        }

        protected PedidoItem()
        {
        }
    }
}