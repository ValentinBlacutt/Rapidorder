namespace Restaurante.Models
{
    public class Bebida : Producto
    {
        public int Mililitros { get; set; }
        public Tamanio Tamanio { get; set; }
        public bool EsAlcoholica { get; set; }

        public decimal PrecioPequeno { get; set; }
        public decimal PrecioMediano { get; set; }
        public decimal PrecioGrande { get; set; }

        // ✅ SOLUCIÓN: NO hacer override de Precio
        // En su lugar, usar el Precio base y asignarlo desde PrecioMediano cuando sea necesario

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

        // ✅ Método helper para obtener precio por nombre de tamaño
        public decimal ObtenerPrecioPorNombre(string nombreTamanio)
        {
            if (nombreTamanio.Contains("Pequeño", StringComparison.OrdinalIgnoreCase))
            if (nombreTamanio.Contains("Pequeño", StringComparison.OrdinalIgnoreCase))
                return PrecioPequeno;
            if (nombreTamanio.Contains("Grande", StringComparison.OrdinalIgnoreCase))
                return PrecioGrande;
            return PrecioMediano;
        }
    }
}