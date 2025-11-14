// En Models/ViewModels/PersonalizarPapasViewModel.cs
using Restaurante.Models;

namespace Restaurante.Models.ViewModels
{
    public class PersonalizarPapasViewModel
    {
        public int PapasId { get; set; }
        public Papas Papas { get; set; }
        public Tamanio TamanioSeleccionado { get; set; }
        public bool ConSal { get; set; } = true;
        public string TipoSalsaSeleccionada { get; set; }
        public decimal PrecioFinal { get; set; }
        public string NotasAdicionales { get; set; }

        // Información de tamaños disponibles
        public Dictionary<Tamanio, string> TamaniosDisponibles => new()
    {
        { Tamanio.Pequeno, $"Pequeño - S/. {Papas?.PrecioPequeno.ToString("0.00") ?? "0.00"}" },
        { Tamanio.Mediano, $"Mediano - S/. {Papas?.PrecioMediano.ToString("0.00") ?? "0.00"}" },
        { Tamanio.Grande, $"Grande - S/. {Papas?.PrecioGrande.ToString("0.00") ?? "0.00"}" }
    };

        // Opciones de salsa
        public List<string> SalsasDisponibles => new()
    {
        "Kétchup",
        "Mayonesa",
        "Mostaza",
        "Salsa de la casa",
        "Salsa BBQ",
        "Sin salsa"
    };

        public string PreciosJson => System.Text.Json.JsonSerializer.Serialize(new
        {
            Pequeno = Papas?.PrecioPequeno ?? 0,
            Mediano = Papas?.PrecioMediano ?? 0,
            Grande = Papas?.PrecioGrande ?? 0
        });
    }
}
