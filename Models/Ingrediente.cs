namespace Restaurante.Models
{
    public class Ingrediente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioAdicional { get; set; }
        public bool Disponible { get; set; } = true;
        public List<IngredienteHamburguesa> Hamburguesas { get; set; } = new();
    }
}
