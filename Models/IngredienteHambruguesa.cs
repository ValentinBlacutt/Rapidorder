namespace Restaurante.Models
{

    public class IngredienteHamburguesa
    {
        public int Id { get; set; } 
        public int HamburguesaId { get; set; }
        public Hamburguesa Hamburguesa { get; set; }

        public int IngredienteId { get; set; }
        public Ingrediente Ingrediente { get; set; }

        public bool EsExtra { get; set; }
        public bool esObligatorio { get; set; } //esto se hace para que estos items no se puedan quitar
        public int Orden { get; set; } // Para ordenar los ingredientes
    }
}
