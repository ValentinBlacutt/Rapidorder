namespace Restaurante.Models.ViewModels
{
    // En Models/ViewModels/PersonalizarComboViewModel.cs
    public class PersonalizarComboViewModel
    {
        public int ComboId { get; set; }
        public Combo Combo { get; set; }
        public List<ItemComboPersonalizable> Items { get; set; } = new();
        public decimal PrecioFinal { get; set; }

        //Esto permite la seleccion de bebida
        public int BebidaSeleccionadaId { get; set; }
        public List<Bebida> BebidasDisponibles { get; set; } = new();
    }

    public class ItemComboPersonalizable
    {
        public int ItemComboId { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public string ProductoDescripcion { get; set; }
        public string ProductoImagen { get; set; }
        public CategoriaProducto Categoria { get; set; }

        // Precios
        public decimal PrecioBase { get; set; }
        public decimal PrecioPersonalizado { get; set; }

        // ✅ INGREDIENTES (para hamburguesas)
        public List<Ingrediente> IngredientesObligatorios { get; set; } = new();
        public List<Ingrediente> IngredientesOpcionalesBase { get; set; } = new();
        public List<Ingrediente> IngredientesDisponibles { get; set; } = new();

        // ✅ NUEVO: Lista de ingredientes opcionales que el usuario MANTIENE
        public List<int> IngredientesOpcionalesSeleccionados { get; set; } = new();
        public List<int> IngredientesExtraSeleccionados { get; set; } = new();

        // Tamaños (para bebidas y papas)
        public Tamanio TamanioSeleccionado { get; set; }

        // Papas
        public bool ConSal { get; set; }
        public string SalsaSeleccionada { get; set; }

        // Notas
        public string NotasPersonalizacion { get; set; }



        // ✅ NUEVO: Para identificar si es el item de bebida
        public bool EsBebida => Categoria == CategoriaProducto.Bebida;

        // ✅ NUEVO: Solo para bebidas - recargos
        public decimal RecargoPequeno { get; set; }
        public decimal RecargoMediano { get; set; }
        public decimal RecargoGrande { get; set; }
    }
}