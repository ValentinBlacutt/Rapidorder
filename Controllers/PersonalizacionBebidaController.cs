using Microsoft.AspNetCore.Mvc;
using Restaurante.Data;
using Restaurante.Models;
using Restaurante.Models.ViewModels;

namespace Restaurante.Controllers
{
    public class PersonalizacionBebidaController : Controller
    {
        private readonly RestauranteContext _context;

        public PersonalizacionBebidaController(RestauranteContext context)
        {
            _context = context;
        }

        public IActionResult PersonalizarBebida(int id)
        {
            var bebida = _context.Bebidas.Find(id);
            if (bebida == null)
                return NotFound();

            var viewModel = new PersonalizarBebidaViewModel
            {
                BebidaId = bebida.Id,
                Bebida = bebida,
                TamanioSeleccionado = Tamanio.Mediano, // Tamaño por defecto
                PrecioFinal = bebida.PrecioMediano, // Precio mediano por defecto
                NotasAdicionales = ""
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ProcesarPersonalizacionBebida(int bebidaId, string tamanioSeleccionado, string notasAdicionales)
        {
            try
            {
                Console.WriteLine($"\n🔹 PROCESANDO PERSONALIZACIÓN BEBIDA");
                Console.WriteLine($"   BebidaId: {bebidaId}");
                Console.WriteLine($"   TamanioSeleccionado (string): {tamanioSeleccionado}");
                Console.WriteLine($"   NotasAdicionales: {notasAdicionales}");

                var bebida = _context.Bebidas.Find(bebidaId);
                if (bebida == null)
                {
                    Console.WriteLine($"   ❌ Bebida no encontrada");
                    TempData["Error"] = "Bebida no encontrada";
                    return RedirectToAction("Index", "Home");
                }

                Console.WriteLine($"   ✅ Bebida encontrada: {bebida.Nombre}");
                Console.WriteLine($"      Precios: P={bebida.PrecioPequeno}, M={bebida.PrecioMediano}, G={bebida.PrecioGrande}");

                // ✅ CONVERTIR STRING A ENUM
                Tamanio tamanio;
                if (!Enum.TryParse<Tamanio>(tamanioSeleccionado, out tamanio))
                {
                    Console.WriteLine($"   ⚠️ No se pudo parsear tamaño, usando Mediano");
                    tamanio = Tamanio.Mediano;
                }

                Console.WriteLine($"   📏 Tamaño parseado: {tamanio}");

                // ✅ CALCULAR PRECIO SEGÚN TAMAÑO
                decimal precioFinal = bebida.ObtenerPrecioPorTamanio(tamanio);
                Console.WriteLine($"   💰 Precio calculado: {precioFinal}");

                // ✅ CONSTRUIR NOTAS
                string notasFinal = $"Tamaño: {tamanio}";
                if (!string.IsNullOrWhiteSpace(notasAdicionales))
                {
                    notasFinal += $" | {notasAdicionales.Trim()}";
                }

                Console.WriteLine($"   📝 Notas finales: {notasFinal}");

                // ✅ REDIRIGIR AL CARRITO CON DATOS CORRECTOS
                Console.WriteLine($"   ➡️ Redirigiendo a CarritoController");
                Console.WriteLine($"🔹 FIN PROCESANDO\n");

                return RedirectToAction("AgregarProductoPersonalizado", "Carrito", new
                {
                    productoId = bebidaId,
                    precioPersonalizado = precioFinal,
                    notas = notasFinal
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR en ProcesarPersonalizacionBebida: {ex.Message}");
                Console.WriteLine($"   Stack: {ex.StackTrace}");
                TempData["Error"] = "Error al procesar la personalización";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}