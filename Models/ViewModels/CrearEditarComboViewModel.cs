// Models/ViewModels/CrearEditarComboViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Restaurante.Models.ViewModels
{
    // En CrearEditarComboViewModel
    public class CrearEditarComboViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal Precio { get; set; }

        public string? ImagenUrl { get; set; }
        public bool Disponible { get; set; } = true;

        // ✅ ESTA ES LA PROPIEDAD IMPORTANTE
        public List<ItemComboViewModel> ProductosDelCombo { get; set; } = new();
        public List<BebidaComboViewModel> BebidasDisponibles { get; set; } = new();

        // Para cargar opciones en el formulario
        public List<Hamburguesa> HamburguesasDisponibles { get; set; } = new();
        public List<Papas> PapasDisponibles { get; set; } = new();
        public List<Bebida> TodasLasBebidas { get; set; } = new();
    }

    // ✅ ViewModel para representar un producto en el combo con su cantidad
    public class ItemComboViewModel
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;

        [Range(0, 10, ErrorMessage = "La cantidad debe estar entre 0 y 10")]
        public int Cantidad { get; set; }
    }

    // ViewModel para las bebidas disponibles en el combo
    public class BebidaComboViewModel
    {
        public int BebidaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Seleccionada { get; set; }

        [Range(-100, 100, ErrorMessage = "El recargo debe estar entre -100 y 100")]
        public decimal RecargoPequeno { get; set; } = -1.00m;

        [Range(-100, 100, ErrorMessage = "El recargo debe estar entre -100 y 100")]
        public decimal RecargoMediano { get; set; } = 0.00m;

        [Range(-100, 100, ErrorMessage = "El recargo debe estar entre -100 y 100")]
        public decimal RecargoGrande { get; set; } = 2.00m;
    }
}