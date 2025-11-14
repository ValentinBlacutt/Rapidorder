// En Models/Pedido.cs
namespace Restaurante.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string NumeroPedido { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        public string ClienteNombre { get; set; }
        public TipoPedido TipoPedido { get; set; }

        //  Método de pago
        public MetodoPago MetodoPago { get; set; }

        //  Estado del pago
        public EstadoPago EstadoPago { get; set; } = EstadoPago.Pendiente;

        public string Notas { get; set; }
        public DateTime FechaPedido { get; set; } = DateTime.Now;

        //  Fecha cuando se realizó el pago
        public DateTime? FechaPago { get; set; }

        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;
        public List<ItemPedido> Items { get; set; } = new();

        private decimal _subtotal;
        public decimal Subtotal
        {
            get => _subtotal != 0 ? _subtotal : Items.Sum(item => item.Subtotal);
            set => _subtotal = value;
        }
        public decimal Impuestos => Subtotal * 0.18m;
        public decimal Total => Subtotal + Impuestos;
    }

   


}