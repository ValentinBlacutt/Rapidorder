using System.ComponentModel.DataAnnotations;

namespace Restaurante.Models.ViewModels
{
    // En Models/ViewModels/HomeViewModel.cs
    public class HomeViewModel
    {
        public List<Producto> ProductosDestacados { get; set; }
        public List<Combo> Combos { get; set; }
    }

    // En Models/ViewModels/MenuViewModel.cs
    public class MenuViewModel
    {
        public List<Hamburguesa> Hamburguesas { get; set; }
        public List<Papas> Papas { get; set; }
        public List<Bebida> Bebidas { get; set; }
    }

}
