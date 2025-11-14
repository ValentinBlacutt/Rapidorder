using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurante.Models
{
    public class ItemCarrito
    {
        // Propiedades para Session (sin mapear a BD)
        [NotMapped] // 
        public Producto? Producto { get; set; }

        [NotMapped] // ← ESTA LÍNEA ES IMPORTANTE
        public Combo? Combo { get; set; }

        // Propiedades para BD y Session
        public int? ProductoId { get; set; }
        public int? ComboId { get; set; }
        public int Cantidad { get; set; }
        public string Notas { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }

        [NotMapped] 
        public decimal Subtotal => PrecioUnitario * Cantidad;

        // Propiedades para BD (se mantienen pero no se usan en Session)
        public int Id { get; set; }
        public int CarritoId { get; set; }
        public Carrito Carrito { get; set; } = null!;
    }
}