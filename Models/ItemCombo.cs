namespace Restaurante.Models
{

    public class ItemCombo
    {
        public int Id { get; set; }
        public int ComboId { get; set; }
        public Combo Combo { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
    }
}
