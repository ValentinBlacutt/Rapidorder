namespace Restaurante.Models
{
    
    public class Papas : Producto
    {
        public Tamanio Tamanio { get; set; }
        public string TipoSalsa { get; set; }

        // Precios por tamaño
        public decimal PrecioPequeno { get; set; }
        public decimal PrecioMediano { get; set; }
        public decimal PrecioGrande { get; set; }

        // Método para obtener precio según tamaño
        public decimal ObtenerPrecioPorTamanio(Tamanio tamanio)
        {
            return tamanio switch
            {
                Tamanio.Pequeno => PrecioPequeno,
                Tamanio.Mediano => PrecioMediano,
                Tamanio.Grande => PrecioGrande,
                _ => PrecioMediano
            };
        }
    }
}
