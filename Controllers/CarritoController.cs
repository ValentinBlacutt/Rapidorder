using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurante.Data;
using Restaurante.Models;
using System.Text.Json;

namespace Restaurante.Controllers
{
    public class CarritoController : Controller
    {
        private readonly RestauranteContext _context;
        private const string CarritoSessionKey = "CarritoData";

        public CarritoController(RestauranteContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();

            // Cargar información completa de productos/combos
            foreach (var item in carrito.Items)
            {
                if (item.ProductoId.HasValue)
                {
                    var producto = _context.Productos
                        .OfType<Bebida>()
                        .FirstOrDefault(p => p.Id == item.ProductoId.Value) as Producto
                        ?? _context.Productos
                        .OfType<Papas>()
                        .FirstOrDefault(p => p.Id == item.ProductoId.Value) as Producto
                        ?? _context.Productos
                        .OfType<Hamburguesa>()
                        .FirstOrDefault(p => p.Id == item.ProductoId.Value) as Producto
                        ?? _context.Productos.Find(item.ProductoId.Value);

                    item.Producto = producto;
                }
                if (item.ComboId.HasValue)
                {
                    item.Combo = _context.Combos.Find(item.ComboId.Value);
                }
            }

            return View(carrito);
        }

        [HttpPost]
        public IActionResult AgregarProducto(int productoId, int cantidad = 1, string notas = "")
        {
            var carrito = ObtenerCarrito();

            var producto = _context.Productos
                .OfType<Bebida>()
                .FirstOrDefault(p => p.Id == productoId) as Producto
                ?? _context.Productos
                .OfType<Papas>()
                .FirstOrDefault(p => p.Id == productoId) as Producto
                ?? _context.Productos.Find(productoId);

            if (producto == null || !producto.Disponible)
                return Json(new { success = false, message = "Producto no disponible" });

            decimal precioUnitario;
            string notasFinal = string.IsNullOrEmpty(notas) ? "Sin personalización" : notas;

            if (producto is Bebida bebida)
            {
                precioUnitario = bebida.PrecioMediano;
                if (string.IsNullOrEmpty(notas))
                {
                    notasFinal = "Tamaño: Mediano";
                }
            }
            else if (producto is Papas papas)
            {
                precioUnitario = papas.PrecioMediano;
                if (string.IsNullOrEmpty(notas))
                {
                    notasFinal = "Tamaño: Mediano";
                }
            }
            else
            {
                precioUnitario = producto.Precio;
            }

            var itemExistente = carrito.Items.FirstOrDefault(i =>
                i.ProductoId == productoId &&
                i.Notas == notasFinal &&
                i.PrecioUnitario == precioUnitario);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Items.Add(new ItemCarrito
                {
                    ProductoId = productoId,
                    Cantidad = cantidad,
                    Notas = notasFinal,
                    PrecioUnitario = precioUnitario
                });
            }

            GuardarCarrito(carrito);
            return Json(new { success = true, totalItems = carrito.Items.Sum(i => i.Cantidad) });
        }

        [HttpPost]
        public IActionResult ActualizarCantidad(int itemIndex, int cantidad)
        {
            var carrito = ObtenerCarrito();

            if (itemIndex >= 0 && itemIndex < carrito.Items.Count)
            {
                if (cantidad <= 0)
                {
                    carrito.Items.RemoveAt(itemIndex);
                }
                else
                {
                    carrito.Items[itemIndex].Cantidad = cantidad;
                }

                GuardarCarrito(carrito);
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult EliminarItem(int itemIndex)
        {
            var carrito = ObtenerCarrito();

            if (itemIndex >= 0 && itemIndex < carrito.Items.Count)
            {
                carrito.Items.RemoveAt(itemIndex);
                GuardarCarrito(carrito);
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult LimpiarCarrito()
        {
            HttpContext.Session.Remove(CarritoSessionKey);
            return Json(new { success = true });
        }

        public IActionResult GetCartCount()
        {
            var carrito = ObtenerCarrito();
            var totalItems = carrito.Items.Sum(i => i.Cantidad);
            return Json(totalItems);
        }

        private Carrito ObtenerCarrito()
        {
            var carritoJson = HttpContext.Session.GetString(CarritoSessionKey);

            if (!string.IsNullOrEmpty(carritoJson))
            {
                try
                {
                    var carrito = JsonSerializer.Deserialize<Carrito>(carritoJson);
                    if (carrito != null)
                    {
                        // Asegurar que Items no sea null
                        if (carrito.Items == null)
                        {
                            carrito.Items = new List<ItemCarrito>();
                        }
                        return carrito;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error deserializando carrito: {ex.Message}");
                }
            }

            return new Carrito { Items = new List<ItemCarrito>() };
        }

        private void GuardarCarrito(Carrito carrito)
        {
            try
            {
                // Asegurar que Items no sea null antes de serializar
                if (carrito.Items == null)
                {
                    carrito.Items = new List<ItemCarrito>();
                }

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
                };

                var carritoJson = JsonSerializer.Serialize(carrito, options);

                // Guardar en sesión
                HttpContext.Session.SetString(CarritoSessionKey, carritoJson);

                Console.WriteLine($"💾 Carrito guardado: {carrito.Items.Count} items");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error guardando carrito: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult AgregarProductoPersonalizado(int productoId, decimal? precioPersonalizado = null, string notas = "")
        {
            try
            {
                Console.WriteLine($"\n🔹 INICIO AgregarProductoPersonalizado");
                Console.WriteLine($"   ProductoId: {productoId}");
                Console.WriteLine($"   PrecioPersonalizado: {precioPersonalizado}");
                Console.WriteLine($"   Notas: {notas}");

                // OBTENER CARRITO ACTUAL
                var carrito = ObtenerCarrito();
                Console.WriteLine($"   Items en carrito antes: {carrito.Items.Count}");

                //  CARGAR PRODUCTO
                var producto = _context.Productos
                    .OfType<Bebida>()
                    .FirstOrDefault(p => p.Id == productoId) as Producto
                    ?? _context.Productos
                    .OfType<Papas>()
                    .FirstOrDefault(p => p.Id == productoId) as Producto
                    ?? _context.Productos.Find(productoId);

                if (producto == null || !producto.Disponible)
                {
                    Console.WriteLine($"   ❌ Producto no encontrado o no disponible");
                    TempData["Error"] = "Producto no disponible";
                    return RedirectToAction("Index", "Home");
                }

                Console.WriteLine($"   ✅ Producto encontrado: {producto.Nombre}");

                decimal precioFinal;
                string notasFinal = notas;

                //  PROCESAR SEGÚN TIPO DE PRODUCTO
                if (producto is Bebida bebida)
                {
                    // Si viene precio personalizado, usarlo; sino, usar mediano por defecto
                    if (precioPersonalizado.HasValue && precioPersonalizado.Value > 0)
                    {
                        precioFinal = precioPersonalizado.Value;
                    }
                    else
                    {
                        precioFinal = bebida.PrecioMediano;
                    }

                    Console.WriteLine($"   💧 Es bebida - Precio final: {precioFinal}");
                    Console.WriteLine($"      Pequeño: {bebida.PrecioPequeno}, Mediano: {bebida.PrecioMediano}, Grande: {bebida.PrecioGrande}");

                    // Extraer o agregar información de tamaño
                    if (string.IsNullOrEmpty(notasFinal) || !notasFinal.Contains("Tamaño:"))
                    {
                        string tamañoInfo = ObtenerTamañoBebida(precioFinal, bebida);
                        notasFinal = string.IsNullOrEmpty(notasFinal)
                            ? tamañoInfo
                            : $"{tamañoInfo} | {notasFinal}";
                    }
                }
                else if (producto is Papas papas)
                {
                    if (precioPersonalizado.HasValue && precioPersonalizado.Value > 0)
                    {
                        precioFinal = precioPersonalizado.Value;
                    }
                    else
                    {
                        precioFinal = papas.PrecioMediano;
                    }

                    Console.WriteLine($"   🍟 Es papas - Precio final: {precioFinal}");

                    if (string.IsNullOrEmpty(notasFinal) || !notasFinal.Contains("Tamaño:"))
                    {
                        string tamañoInfo = ObtenerTamañoPapas(precioFinal, papas);
                        notasFinal = string.IsNullOrEmpty(notasFinal)
                            ? tamañoInfo
                            : $"{tamañoInfo} | {notasFinal}";
                    }
                }
                else
                {
                    precioFinal = precioPersonalizado ?? producto.Precio;
                    Console.WriteLine($"   🍔 Producto normal - Precio final: {precioFinal}");
                }

                if (string.IsNullOrEmpty(notasFinal))
                {
                    notasFinal = "Sin personalización";
                }

                Console.WriteLine($"   📝 Notas finales: {notasFinal}");
                Console.WriteLine($"   💰 Precio final: {precioFinal}");

                // ✅ BUSCAR ITEM EXISTENTE CON MISMAS CARACTERÍSTICAS
                var itemExistente = carrito.Items.FirstOrDefault(i =>
                    i.ProductoId == productoId &&
                    Math.Abs(i.PrecioUnitario - precioFinal) < 0.01m &&
                    i.Notas == notasFinal);

                if (itemExistente != null)
                {
                    Console.WriteLine($"   ♻️ Item existente encontrado, incrementando cantidad");
                    itemExistente.Cantidad += 1;
                }
                else
                {
                    Console.WriteLine($"   ➕ Agregando nuevo item al carrito");
                    var nuevoItem = new ItemCarrito
                    {
                        ProductoId = productoId,
                        Cantidad = 1,
                        Notas = notasFinal,
                        PrecioUnitario = precioFinal
                    };

                    carrito.Items.Add(nuevoItem);
                    Console.WriteLine($"      Item agregado - Subtotal: {nuevoItem.Subtotal}");
                }

                Console.WriteLine($"   Items en carrito después: {carrito.Items.Count}");

                
                GuardarCarrito(carrito);

              
                var carritoVerificacion = ObtenerCarrito();
                Console.WriteLine($"   ✔️ Verificación: {carritoVerificacion.Items.Count} items guardados");

                string mensaje = precioPersonalizado.HasValue ? "personalizado agregado" : "agregado";
                TempData["Success"] = $"¡{producto.Nombre} {mensaje} al carrito!";

                Console.WriteLine($"🔹 FIN AgregarProductoPersonalizado\n");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR en AgregarProductoPersonalizado: {ex.Message}");
                Console.WriteLine($"   Stack: {ex.StackTrace}");
                TempData["Error"] = "Error al agregar producto al carrito";
                return RedirectToAction("Index", "Home");
            }
        }

        private string ObtenerTamañoPapas(decimal precio, Papas papas)
        {
            // Comparar con tolerancia para decimales
            if (Math.Abs(precio - papas.PrecioPequeno) < 0.01m) return "Tamaño: Pequeño";
            if (Math.Abs(precio - papas.PrecioMediano) < 0.01m) return "Tamaño: Mediano";
            if (Math.Abs(precio - papas.PrecioGrande) < 0.01m) return "Tamaño: Grande";
            return "Tamaño: Mediano";
        }

        private string ObtenerTamañoBebida(decimal precio, Bebida bebida)
        {
            // Comparar con tolerancia para decimales
            if (Math.Abs(precio - bebida.PrecioPequeno) < 0.01m) return "Tamaño: Pequeño";
            if (Math.Abs(precio - bebida.PrecioMediano) < 0.01m) return "Tamaño: Mediano";
            if (Math.Abs(precio - bebida.PrecioGrande) < 0.01m) return "Tamaño: Grande";
            return "Tamaño: Mediano";
        }

        [HttpGet]
        public IActionResult AgregarComboPersonalizado(int comboId, decimal precioPersonalizado, string notas)
        {
            try
            {
                var carrito = ObtenerCarrito();
                var combo = _context.Combos.Find(comboId);

                if (combo == null || !combo.Disponible)
                {
                    TempData["Error"] = "Combo no disponible";
                    return RedirectToAction("Index", "Home");
                }

                carrito.Items.Add(new ItemCarrito
                {
                    ComboId = comboId,
                    Cantidad = 1,
                    Notas = string.IsNullOrEmpty(notas) ? "Sin personalización" : notas,
                    PrecioUnitario = precioPersonalizado
                });

                GuardarCarrito(carrito);

                TempData["Success"] = $"¡{combo.Nombre} personalizado agregado al carrito!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Error al agregar combo al carrito";
                return RedirectToAction("Index", "Home");
            }
        }

        // ✅ MÉTODO DE DEBUG
        [HttpGet]
        public IActionResult DebugCarrito()
        {
            var carritoJson = HttpContext.Session.GetString(CarritoSessionKey);
            var carrito = ObtenerCarrito();

            var html = $@"
                <h2>🔍 DEBUG CARRITO</h2>
                <h3>Items en carrito: {carrito.Items.Count}</h3>
                <ul>
            ";

            foreach (var item in carrito.Items)
            {
                html += $@"
                    <li>
                        ProductoId: {item.ProductoId}<br/>
                        Cantidad: {item.Cantidad}<br/>
                        PrecioUnitario: {item.PrecioUnitario}<br/>
                        Subtotal: {item.Subtotal}<br/>
                        Notas: {item.Notas}
                    </li>
                ";
            }

            html += $@"
                </ul>
                <h3>JSON Raw:</h3>
                <pre>{carritoJson ?? "NULL"}</pre>
            ";

            return Content(html, "text/html");
        }
    }
}