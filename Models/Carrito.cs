using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurante.Models
{
    public class Carrito
    {
        // Propiedades para Session (sin mapear a BD)
        [NotMapped] // ← ESTA LÍNEA ES IMPORTANTE
        public List<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();

        [NotMapped] // ← ESTA LÍNEA ES IMPORTANTE
        public decimal Total => Items.Sum(item => item.Subtotal);

        // Propiedades para BD (se mantienen pero no se usan en Session)
        public int Id { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}