using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurante.Data;
using Restaurante.Models;
using Restaurante.Models.ViewModels;
using System.Text.Json;

namespace Restaurante.Controllers
{
    public class PedidoController : Controller
    {
        private readonly RestauranteContext _context;
        private const string CarritoSessionKey = "CarritoData";

        public PedidoController(RestauranteContext context)
        {
            _context = context;
        }

        public IActionResult Checkout()
        {
            var carrito = ObtenerCarrito();

            if (carrito == null || !carrito.Items.Any())
            {
                TempData["Error"] = "Tu carrito está vacío";
                return RedirectToAction("Index", "Carrito");
            }

            var model = new PedidoViewModel
            {
                Carrito = carrito
            };

            TempData["CarritoData"] = JsonSerializer.Serialize(carrito);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(PedidoViewModel model)
        {
            if (TempData["CarritoData"] != null)
            {
                var carritoJson = TempData["CarritoData"].ToString();
                model.Carrito = JsonSerializer.Deserialize<Carrito>(carritoJson);
                TempData.Keep("CarritoData");
            }
            else
            {
                model.Carrito = ObtenerCarrito();
            }

            if (model.Carrito == null || !model.Carrito.Items.Any())
            {
                TempData["Error"] = "Tu carrito está vacío";
                return RedirectToAction("Index", "Carrito");
            }

            // ✅ CORREGIDO: Remover validación de Notas del ModelState
            ModelState.Remove("Notas");

            // ✅ CORREGIDO: Asegurar que Notas no sea null
            if (string.IsNullOrWhiteSpace(model.Notas))
            {
                model.Notas = string.Empty;
            }

            if (!ModelState.IsValid)
            {
                TempData.Keep("CarritoData");
                return View("Checkout", model);
            }

            try
            {
                var pedido = new Pedido
                {
                    ClienteNombre = model.ClienteNombre,
                    TipoPedido = model.TipoPedido,
                    MetodoPago = model.MetodoPago,
                    Notas = model.Notas ?? string.Empty, // ✅ Asegurar que nunca sea null
                    Estado = EstadoPedido.Pendiente,
                    EstadoPago = EstadoPago.Pendiente,
                    FechaPedido = DateTime.Now
                };

                foreach (var item in model.Carrito.Items)
                {
                    var producto = item.ProductoId.HasValue ? _context.Productos.Find(item.ProductoId.Value) : null;
                    var combo = item.ComboId.HasValue ? _context.Combos.Find(item.ComboId.Value) : null;

                    var itemPedido = new ItemPedido
                    {
                        NombreProducto = producto?.Nombre ?? combo?.Nombre ?? "Producto",
                        Descripcion = producto?.Descripcion ?? combo?.Descripcion ?? "",
                        PrecioUnitario = item.PrecioUnitario,
                        Cantidad = item.Cantidad,
                        Notas = item.Notas ?? string.Empty, // ✅ Asegurar que nunca sea null
                        ProductoId = item.ProductoId,
                        ComboId = item.ComboId
                    };
                    pedido.Items.Add(itemPedido);
                }

                pedido.Subtotal = pedido.Items.Sum(i => i.Subtotal);

                _context.Pedidos.Add(pedido);
                _context.SaveChanges();

                HttpContext.Session.Remove(CarritoSessionKey);
                TempData.Remove("CarritoData");

                TempData["Success"] = "Pedido creado exitosamente";
                return RedirectToAction("Pago", new { id = pedido.Id });
            }
            catch (Exception ex)
            {
                TempData.Keep("CarritoData");
                TempData["Error"] = "Error al procesar el pedido. Inténtalo de nuevo.";
                return View("Checkout", model);
            }
        }

        // GET: Página de pago
        public IActionResult Pago(int id)
        {
            var pedido = _context.Pedidos
                .Include(p => p.Items)
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null)
                return NotFound();

            return View(pedido);
        }

        // POST: Confirmar pago
        [HttpPost]
        public IActionResult ConfirmarPago(int id)
        {
            var pedido = _context.Pedidos
                .Include(p => p.Items)
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null)
                return NotFound();

            // ✅ CORREGIDO: Diferenciar entre pago online y pago en mostrador
            if (pedido.MetodoPago == MetodoPago.PagoOnline)
            {
                // PAGO ONLINE: Marcar como pagado y confirmado inmediatamente
                pedido.EstadoPago = EstadoPago.Pagado;
                pedido.Estado = EstadoPedido.Confirmado;
                pedido.FechaPago = DateTime.Now;
            }
            else
            {
                // PAGO EN MOSTRADOR: Mantener como pendiente
                // El admin lo marcará como pagado manualmente en el mostrador
                pedido.EstadoPago = EstadoPago.Pendiente;
                pedido.Estado = EstadoPedido.Pendiente;
            }

            _context.SaveChanges();

            return RedirectToAction("Confirmacion", new { id = pedido.Id });
        }

        // GET: Página de confirmación
        public IActionResult Confirmacion(int id)
        {
            var pedido = _context.Pedidos
                .Include(p => p.Items)
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null)
                return NotFound();

            return View(pedido);
        }

        private Carrito ObtenerCarrito()
        {
            var carritoJson = HttpContext.Session.GetString(CarritoSessionKey);
            if (!string.IsNullOrEmpty(carritoJson))
            {
                return JsonSerializer.Deserialize<Carrito>(carritoJson) ?? new Carrito();
            }
            return new Carrito();
        }
    }
}