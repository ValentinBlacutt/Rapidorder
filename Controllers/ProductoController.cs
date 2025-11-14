using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurante.Data;
using Restaurante.Models;

namespace Restaurante.Controllers
{
    // En Controllers/ProductoController.cs
    public class ProductoController : Controller
    {
        private readonly RestauranteContext _context;

        public ProductoController(RestauranteContext context)
        {
            _context = context;
        }

        public IActionResult Detalle(int id)
        {
            var producto = _context.Productos
                .Include(p => (p as Hamburguesa).Ingredientes)
                .ThenInclude(ih => ih.Ingrediente)
                .FirstOrDefault(p => p.Id == id);

            if (producto == null)
                return NotFound();

            return View(producto);
        }
    }
}
