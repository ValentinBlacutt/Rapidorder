namespace Restaurante.Models.ViewModels
{
    public class PersonalizarHamburguesaViewModel
    {
        public int HamburguesaId { get; set; }
        public Hamburguesa Hamburguesa { get; set; }

        // ✅ Ingredientes OBLIGATORIOS (no se pueden quitar)
        public List<Ingrediente> IngredientesObligatorios { get; set; }

        // ✅ Ingredientes base OPCIONALES (incluidos pero se pueden quitar)
        public List<Ingrediente> IngredientesOpcionalesBase { get; set; }

        // ✅ Ingredientes EXTRAS disponibles para agregar
        public List<Ingrediente> IngredientesDisponibles { get; set; }

        public List<int> IngredientesExtra { get; set; }
        public string NotasAdicionales { get; set; }
    }
}