// En Controllers/PersonalizacionPapasController.cs
using Microsoft.AspNetCore.Mvc;
using Restaurante.Data;
using Restaurante.Models;
using Restaurante.Models.ViewModels;

namespace Restaurante.Controllers
{
    public class PersonalizacionPapasController : Controller
    {
        private readonly RestauranteContext _context;

        public PersonalizacionPapasController(RestauranteContext context)
        {
            _context = context;
        }

        public IActionResult PersonalizarPapas(int id)
        {
            var papas = _context.Papas.Find(id);

            if (papas == null)
                return NotFound();

            var viewModel = new PersonalizarPapasViewModel
            {
                PapasId = papas.Id,
                Papas = papas,
                TamanioSeleccionado = papas.Tamanio,
                ConSal = true,
                TipoSalsaSeleccionada = papas.TipoSalsa ?? "Kétchup",
                PrecioFinal = papas.Precio,
                NotasAdicionales = ""
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ProcesarPersonalizacionPapas(int papasId, string tamanioSeleccionado, bool conSal, string tipoSalsaSeleccionada, string notasAdicionales)
        {
            var papas = _context.Papas.Find(papasId);

            if (papas == null)
            {
                TempData["Error"] = "Papas no encontradas";
                return RedirectToAction("Index", "Home");
            }

            // Convertir string a enum Tamanio
            if (!Enum.TryParse<Tamanio>(tamanioSeleccionado, out var tamanio))
            {
                tamanio = Tamanio.Mediano; // Valor por defecto
            }

            // Calcular precio según el tamaño seleccionado
            var precioFinal = papas.ObtenerPrecioPorTamanio(tamanio);

            // Crear notas para el pedido
            var notas = $"Tamaño: {tamanio}; ";
            notas += conSal ? "Con sal" : "Sin sal";
            notas += $"; Salsa: {tipoSalsaSeleccionada}";

            if (!string.IsNullOrEmpty(notasAdicionales))
            {
                notas += $"; {notasAdicionales}";
            }

            // Redirigir al método único del CarritoController
            return RedirectToAction("AgregarProductoPersonalizado", "Carrito", new
            {
                productoId = papasId,
                precioPersonalizado = precioFinal,
                notas = notas
            });
        }
    }
}