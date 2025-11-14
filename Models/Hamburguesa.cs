namespace Restaurante.Models
{
    public class Hamburguesa : Producto
    {
        public List<IngredienteHamburguesa> Ingredientes { get; set; } = new();
        public bool EsVegetariana { get; set; }
    }
}
