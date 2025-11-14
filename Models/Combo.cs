using Restaurante.Models;

namespace Restaurante.Models
{
    public class Combo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string ImagenUrl { get; set; }
        public bool Disponible { get; set; }
        public List<ItemCombo> Items { get; set; } = new();

        
        public List<ComboBebida> BebidasDisponibles { get; set; } = new();
    }


}
