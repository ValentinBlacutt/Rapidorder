// En Models/ViewModels/PedidoViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Restaurante.Models.ViewModels
{
    public class PedidoViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string ClienteNombre { get; set; }

        [Required(ErrorMessage = "Debes seleccionar el tipo de pedido")]
        public TipoPedido TipoPedido { get; set; }

        // ✅ NUEVO
        [Required(ErrorMessage = "Debes seleccionar el método de pago")]
        public MetodoPago MetodoPago { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        public string? Notas { get; set; }

        public Carrito Carrito { get; set; } = new Carrito();
    }
}