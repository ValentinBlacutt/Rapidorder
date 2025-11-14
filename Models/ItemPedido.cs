namespace Restaurante.Models
{

    public class ItemPedido
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }

        public string NombreProducto { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public string Notas { get; set; }

        
        private decimal _subtotal;
        public decimal Subtotal
        {
            get => _subtotal != 0 ? _subtotal : PrecioUnitario * Cantidad;
            set => _subtotal = value;
        }

        // Referencias opcionales al producto/combo original
        public int? ProductoId { get; set; }
        public int? ComboId { get; set; }
    }
}
