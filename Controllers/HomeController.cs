// En Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurante.Models;
using Restaurante.Models.ViewModels;
using Restaurante.Data;

public class HomeController : Controller
{
    private readonly RestauranteContext _context;

    public HomeController(RestauranteContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var productosDestacados = _context.Productos
            .Where(p => p.Disponible)
            .Take(8)
            .ToList();

        var combos = _context.Combos
            .Where(c => c.Disponible)
            .Include(c => c.Items)
            .ThenInclude(i => i.Producto)
            .ToList();

        var viewModel = new HomeViewModel
        {
            ProductosDestacados = productosDestacados,
            Combos = combos
        };

        return View(viewModel);
    }

    public IActionResult Menu()
    {
        var hamburguesas = _context.Hamburguesas
            .Include(h => h.Ingredientes)
            .ThenInclude(ih => ih.Ingrediente)
            .Where(h => h.Disponible)
            .ToList();

        var papas = _context.Papas.Where(p => p.Disponible).ToList();
        var bebidas = _context.Bebidas.Where(b => b.Disponible).ToList();

        var viewModel = new MenuViewModel
        {
            Hamburguesas = hamburguesas,
            Papas = papas,
            Bebidas = bebidas
        };

        return View(viewModel);
    }
}



