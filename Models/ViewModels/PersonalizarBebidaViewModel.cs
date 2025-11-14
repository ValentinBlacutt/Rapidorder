namespace Restaurante.Models.ViewModels
{
    // En Models/ViewModels/PersonalizarBebidaViewModel.cs
    // En Models/ViewModels/PersonalizarBebidaViewModel.cs
    public class PersonalizarBebidaViewModel
    {
        public int BebidaId { get; set; }
        public Bebida Bebida { get; set; }
        public Tamanio TamanioSeleccionado { get; set; }
        public decimal PrecioFinal { get; set; }
        public string NotasAdicionales { get; set; }

        // Información de tamaños disponibles
        public Dictionary<Tamanio, string> TamaniosDisponibles => new()
    {
        { Tamanio.Pequeno, $"Pequeño ({GetMililitros(Tamanio.Pequeno)}ml) - S/. {Bebida?.PrecioPequeno.ToString("0.00") ?? "0.00"}" },
        { Tamanio.Mediano, $"Mediano ({GetMililitros(Tamanio.Mediano)}ml) - S/. {Bebida?.PrecioMediano.ToString("0.00") ?? "0.00"}" },
        { Tamanio.Grande, $"Grande ({GetMililitros(Tamanio.Grande)}ml) - S/. {Bebida?.PrecioGrande.ToString("0.00") ?? "0.00"}" }
    };

        // ✅ NUEVO: Propiedades para JavaScript
        public string PreciosJson => System.Text.Json.JsonSerializer.Serialize(new
        {
            Pequeno = Bebida?.PrecioPequeno ?? 0,
            Mediano = Bebida?.PrecioMediano ?? 0,
            Grande = Bebida?.PrecioGrande ?? 0
        });

        public string MililitrosJson => System.Text.Json.JsonSerializer.Serialize(new
        {
            Pequeno = GetMililitros(Tamanio.Pequeno),
            Mediano = GetMililitros(Tamanio.Mediano),
            Grande = GetMililitros(Tamanio.Grande)
        });

        private int GetMililitros(Tamanio tamanio)
        {
            if (Bebida == null) return 0;

            return tamanio switch
            {
                Tamanio.Pequeno => (int)(Bebida.Mililitros * 0.7), // 70% del tamaño mediano
                Tamanio.Mediano => Bebida.Mililitros,
                Tamanio.Grande => (int)(Bebida.Mililitros * 1.3), // 130% del tamaño mediano
                _ => Bebida.Mililitros
            };
        }
    }
}
