namespace Restaurante.Models
{

    // Entidades Principales
    public abstract class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string ImagenUrl { get; set; }
        public bool Disponible { get; set; }
        public CategoriaProducto Categoria { get; set; }
    }
}
