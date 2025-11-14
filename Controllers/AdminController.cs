using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurante.Data;
using Restaurante.Models;
using System.IO;

namespace Restaurante.Controllers
{
    [Authorize(Roles = "SuperAdmin,Cocina")]
    public class AdminController : Controller
    {
        private readonly RestauranteContext _context;

        public AdminController(RestauranteContext context)
        {
            _context = context;
        }

        // GET: /admin/dashboard
        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Dashboard Principal";

            var hoy = DateTime.Today;

            if (User.IsInRole("SuperAdmin"))
            {
                ViewBag.TotalPedidosHoy = await _context.Pedidos
                    .Where(p => p.FechaPedido.Date == hoy)
                    .CountAsync();

                ViewBag.IngresosHoy = await _context.Pedidos
                    .Where(p => p.FechaPedido.Date == hoy && p.Estado != EstadoPedido.Cancelado)
                    .Select(p => p.Subtotal + (p.Subtotal * 0.18m))
                    .SumAsync();

                ViewBag.PedidosPendientes = await _context.Pedidos
                    .Where(p => p.Estado == EstadoPedido.Pendiente || p.Estado == EstadoPedido.Confirmado)
                    .CountAsync();

                ViewBag.PedidosEnPreparacion = await _context.Pedidos
                    .Where(p => p.Estado == EstadoPedido.EnPreparacion)
                    .CountAsync();

                ViewBag.TotalProductos = await _context.Productos
                    .Where(p => p.Disponible)
                    .CountAsync();

                ViewBag.PedidosRecientes = await _context.Pedidos
                    .Include(p => p.Items)
                    .Where(p => p.Estado != EstadoPedido.Entregado && p.Estado != EstadoPedido.Cancelado)
                    .OrderByDescending(p => p.FechaPedido)
                    .Take(5)
                    .ToListAsync();

                return View("DashboardSuperAdmin");
            }
            else
            {
                ViewBag.PedidosPendientes = await _context.Pedidos
                    .Where(p => p.Estado == EstadoPedido.Pendiente || p.Estado == EstadoPedido.Confirmado)
                    .CountAsync();

                ViewBag.PedidosEnPreparacion = await _context.Pedidos
                    .Where(p => p.Estado == EstadoPedido.EnPreparacion)
                    .CountAsync();

                ViewBag.PedidosListos = await _context.Pedidos
                    .Where(p => p.Estado == EstadoPedido.Listo)
                    .CountAsync();

                return View("DashboardCocina");
            }
        }

        // GET: /admin/pedidos
        public async Task<IActionResult> Pedidos(string estado = null, string buscar = null)
        {
            ViewData["Title"] = "Gestión de Pedidos";
            ViewData["EstadoActual"] = estado;
            ViewData["Buscar"] = buscar;

            var query = _context.Pedidos
                .Include(p => p.Items)
                .AsQueryable();

            // Filtro por estado
            if (!string.IsNullOrEmpty(estado) && Enum.TryParse<EstadoPedido>(estado, out var estadoPedido))
            {
                query = query.Where(p => p.Estado == estadoPedido);
            }

            // Búsqueda por número de pedido o nombre de cliente
            if (!string.IsNullOrEmpty(buscar))
            {
                buscar = buscar.Trim().ToUpper();
                query = query.Where(p => 
                    p.NumeroPedido.Contains(buscar) || 
                    p.ClienteNombre.ToUpper().Contains(buscar));
            }

            var pedidos = await query
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            return View(pedidos);
        }

        // POST: /admin/pedidos/marcar-pagado/{id}
        [HttpPost("admin/pedidos/marcar-pagado/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarComoPagado(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            if (pedido.MetodoPago == MetodoPago.PagoEnMostrador &&
                pedido.EstadoPago == EstadoPago.Pendiente)
            {
                pedido.EstadoPago = EstadoPago.Pagado;
                pedido.Estado = EstadoPedido.Confirmado;
                pedido.FechaPago = DateTime.Now;

                _context.Update(pedido);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Pedido marcado como pagado y confirmado";
            }
            else
            {
                TempData["Error"] = "El pedido ya fue pagado o no es pago en mostrador";
            }

            // Redirigir a DetallePedido 
            return RedirectToAction(nameof(DetallePedido), new { id });
        }

        // POST: /admin/pedidos/en-preparacion/{id}
        [HttpPost("admin/pedidos/en-preparacion/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnPreparacion(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            if (pedido.EstadoPago != EstadoPago.Pagado)
            {
                TempData["Error"] = "El pedido debe estar pagado antes de prepararlo";
                return RedirectToAction(nameof(DetallePedido), new { id });
            }

            pedido.Estado = EstadoPedido.EnPreparacion;
            _context.Update(pedido);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Pedido en preparación";
            return RedirectToAction(nameof(DetallePedido), new { id });
        }

        // POST: /admin/pedidos/listo/{id}
        [HttpPost("admin/pedidos/listo/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Listo(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            pedido.Estado = EstadoPedido.Listo;
            _context.Update(pedido);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Pedido listo para entregar";
            return RedirectToAction(nameof(DetallePedido), new { id });
        }

        // POST: /admin/pedidos/entregado/{id}
        [HttpPost("admin/pedidos/entregado/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Entregado(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            pedido.Estado = EstadoPedido.Entregado;
            _context.Update(pedido);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Pedido entregado";
            return RedirectToAction(nameof(DetallePedido), new { id });
        }

        // POST: /admin/pedidos/cancelar/{id}
        [HttpPost("admin/pedidos/cancelar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            pedido.Estado = EstadoPedido.Cancelado;
            _context.Update(pedido);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Pedido cancelado";
            return RedirectToAction(nameof(DetallePedido), new { id });
        }

        
        // GET: /admin/detalle-pedido/{id}
        [HttpGet("admin/detalle-pedido/{id}")]
        public async Task<IActionResult> DetallePedido(int id)
        {
            ViewData["Title"] = "Detalle del Pedido";

            var pedido = await _context.Pedidos
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/DetallePedido.cshtml", pedido);
        }

        [HttpPost("admin/pedidos/actualizar-estado/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarEstado(int id, EstadoPedido nuevoEstado)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            // Validaciones según el flujo del pedido
            if (nuevoEstado == EstadoPedido.EnPreparacion && pedido.EstadoPago != EstadoPago.Pagado)
            {
                TempData["Error"] = "El pedido debe estar pagado antes de prepararlo";
                return RedirectToAction(nameof(DetallePedido), new { id });
            }

            // Actualizar estado
            pedido.Estado = nuevoEstado;
            _context.Update(pedido);
            await _context.SaveChangesAsync();

            // Mensaje de éxito personalizado
            string mensaje = nuevoEstado switch
            {
                EstadoPedido.Confirmado => "Pedido confirmado exitosamente",
                EstadoPedido.EnPreparacion => "Pedido en preparación",
                EstadoPedido.Listo => "Pedido listo para entregar",
                EstadoPedido.Entregado => "Pedido entregado",
                EstadoPedido.Cancelado => "Pedido cancelado",
                _ => "Estado actualizado"
            };

            TempData["Success"] = mensaje;
            return RedirectToAction(nameof(DetallePedido), new { id });
        }


        // GET: /admin/hamburguesas
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Hamburguesas()
        {
            ViewData["Title"] = "Gestión de Hamburguesas";

            var hamburguesas = await _context.Hamburguesas
                .Include(h => h.Ingredientes)
                    .ThenInclude(ih => ih.Ingrediente)
                .Where(h => h.Disponible)
                .ToListAsync();

            return View(hamburguesas);
        }

        // GET: /admin/hamburguesas/crear
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CrearHamburguesa()
        {
            ViewData["Title"] = "Crear Nueva Hamburguesa";
            ViewBag.Ingredientes = await _context.Ingredientes
                .Where(i => i.Disponible)
                .ToListAsync();
            return View();
        }

        // POST: /admin/hamburguesas/crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CrearHamburguesa(
            Hamburguesa hamburguesa,
            List<int> ingredientesIncluidos,
            List<bool> ingredientesObligatorios,
            List<int> ingredientesExtras,
            IFormFile imagen)
        {
            try
            {
                // Remover validaciones de ImagenUrl ya que es opcional
                ModelState.Remove("ImagenUrl");

                if (ModelState.IsValid)
                {
                    // Procesar imagen si fue cargada
                    if (imagen != null && imagen.Length > 0)
                    {
                        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                        var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();

                        if (!extensionesPermitidas.Contains(extension))
                        {
                            ModelState.AddModelError("imagen", "Solo se permiten imágenes JPG, PNG o WEBP");
                            ViewBag.Ingredientes = await _context.Ingredientes
                                .Where(i => i.Disponible)
                                .ToListAsync();
                            return View(hamburguesa);
                        }

                        // Generar nombre único para la imagen
                        var nombreArchivo = $"hamburguesa_{Guid.NewGuid()}{extension}";
                        var rutaImagenes = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                        // Crear carpeta si no existe
                        if (!Directory.Exists(rutaImagenes))
                        {
                            Directory.CreateDirectory(rutaImagenes);
                        }

                        var rutaCompleta = Path.Combine(rutaImagenes, nombreArchivo);

                        // Guardar imagen
                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            await imagen.CopyToAsync(stream);
                        }

                        // Asignar URL relativa al modelo
                        hamburguesa.ImagenUrl = $"/images/{nombreArchivo}";
                    }
                    else if (string.IsNullOrWhiteSpace(hamburguesa.ImagenUrl))
                    {
                        // Imagen por defecto si no se cargó ninguna
                        hamburguesa.ImagenUrl = "/images/hamburguesa-default.jpg";
                    }

                    // Asignar categoría automáticamente
                    hamburguesa.Categoria = CategoriaProducto.Hamburguesa;
                    hamburguesa.Disponible = true;

                    _context.Add(hamburguesa);
                    await _context.SaveChangesAsync();

                    int orden = 0;

                    // 1. Agregar ingredientes INCLUIDOS (base)
                    if (ingredientesIncluidos != null && ingredientesIncluidos.Count > 0)
                    {
                        for (int i = 0; i < ingredientesIncluidos.Count; i++)
                        {
                            var ingredienteHamburguesa = new IngredienteHamburguesa
                            {
                                HamburguesaId = hamburguesa.Id,
                                IngredienteId = ingredientesIncluidos[i],
                                EsExtra = false,
                                esObligatorio = ingredientesObligatorios != null && ingredientesObligatorios.Count > i ? ingredientesObligatorios[i] : false,
                                Orden = orden++
                            };
                            _context.Add(ingredienteHamburguesa);
                        }
                    }

                    // 2. Agregar ingredientes EXTRAS (opcionales con costo adicional)
                    if (ingredientesExtras != null && ingredientesExtras.Count > 0)
                    {
                        foreach (var ingredienteId in ingredientesExtras)
                        {
                            var ingredienteHamburguesa = new IngredienteHamburguesa
                            {
                                HamburguesaId = hamburguesa.Id,
                                IngredienteId = ingredienteId,
                                EsExtra = true,
                                esObligatorio = false,
                                Orden = orden++
                            };
                            _context.Add(ingredienteHamburguesa);
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Hamburguesa creada exitosamente";
                    return RedirectToAction(nameof(Hamburguesas));
                }

                // Si el modelo no es válido, mostrar errores
                ViewBag.Ingredientes = await _context.Ingredientes
                    .Where(i => i.Disponible)
                    .ToListAsync();
                return View(hamburguesa);
            }
            catch (Exception ex)
            {
                // Log del error
                TempData["Error"] = $"Error al crear la hamburguesa: {ex.Message}";
                ViewBag.Ingredientes = await _context.Ingredientes
                    .Where(i => i.Disponible)
                    .ToListAsync();
                return View(hamburguesa);
            }
        }

        // POST: /admin/hamburguesas/eliminar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> EliminarHamburguesa(int id)
        {
            var hamburguesa = await _context.Hamburguesas.FindAsync(id);
            if (hamburguesa == null)
            {
                return NotFound();
            }

            hamburguesa.Disponible = false;
            _context.Update(hamburguesa);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Hamburguesa eliminada exitosamente";
            return RedirectToAction(nameof(Hamburguesas));
        }

        
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> EditarHamburguesa(int id)
        {
            ViewData["Title"] = "Editar Hamburguesa";

            var hamburguesa = await _context.Hamburguesas
                .Include(h => h.Ingredientes)
                    .ThenInclude(ih => ih.Ingrediente)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hamburguesa == null)
            {
                return NotFound();
            }

            ViewBag.Ingredientes = await _context.Ingredientes
                .Where(i => i.Disponible)
                .ToListAsync();

            return View("~/Views/Admin/EditarHamburguesa.cshtml", hamburguesa);
        }

        // POST: /admin/hamburguesas/editar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> EditarHamburguesa(
            int id,
            Hamburguesa hamburguesa,
            List<int> ingredientesIncluidos,
            List<bool> ingredientesObligatorios,
            List<int> ingredientesExtras,
            IFormFile? imagen)
        {
            if (id != hamburguesa.Id)
            {
                return NotFound();
            }

            try
            {
                // Remover validaciones opcionales
                ModelState.Remove("ImagenUrl");
                ModelState.Remove("imagen");

                if (ModelState.IsValid)
                {
                    var hamburguesaExistente = await _context.Hamburguesas
                        .Include(h => h.Ingredientes)
                        .FirstOrDefaultAsync(h => h.Id == id);

                    if (hamburguesaExistente == null)
                    {
                        return NotFound();
                    }

                    // Procesar imagen si fue cargada
                    if (imagen != null && imagen.Length > 0)
                    {
                        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                        var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();

                        if (!extensionesPermitidas.Contains(extension))
                        {
                            ModelState.AddModelError("imagen", "Solo se permiten imágenes JPG, PNG o WEBP");
                            ViewBag.Ingredientes = await _context.Ingredientes
                                .Where(i => i.Disponible)
                                .ToListAsync();
                            return View("~/Views/Admin/EditarHamburguesa.cshtml", hamburguesa);
                        }

                        // Eliminar imagen anterior si existe y no es la por defecto
                        if (!string.IsNullOrEmpty(hamburguesaExistente.ImagenUrl) &&
                            hamburguesaExistente.ImagenUrl != "/images/hamburguesa-default.jpg")
                        {
                            var rutaImagenAnterior = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                hamburguesaExistente.ImagenUrl.TrimStart('/'));

                            if (System.IO.File.Exists(rutaImagenAnterior))
                            {
                                System.IO.File.Delete(rutaImagenAnterior);
                            }
                        }

                        // Guardar nueva imagen
                        var nombreArchivo = $"hamburguesa_{Guid.NewGuid()}{extension}";
                        var rutaImagenes = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                        if (!Directory.Exists(rutaImagenes))
                        {
                            Directory.CreateDirectory(rutaImagenes);
                        }

                        var rutaCompleta = Path.Combine(rutaImagenes, nombreArchivo);

                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            await imagen.CopyToAsync(stream);
                        }

                        hamburguesaExistente.ImagenUrl = $"/images/{nombreArchivo}";
                    }
                    else if (!string.IsNullOrWhiteSpace(hamburguesa.ImagenUrl))
                    {
                        hamburguesaExistente.ImagenUrl = hamburguesa.ImagenUrl;
                    }

                    // Actualizar propiedades básicas
                    hamburguesaExistente.Nombre = hamburguesa.Nombre;
                    hamburguesaExistente.Descripcion = hamburguesa.Descripcion;
                    hamburguesaExistente.Precio = hamburguesa.Precio;

                    // Eliminar todas las relaciones de ingredientes anteriores
                    _context.RemoveRange(hamburguesaExistente.Ingredientes);
                    await _context.SaveChangesAsync();

                    int orden = 0;

                    // 1. Agregar ingredientes INCLUIDOS (base)
                    if (ingredientesIncluidos != null && ingredientesIncluidos.Count > 0)
                    {
                        for (int i = 0; i < ingredientesIncluidos.Count; i++)
                        {
                            var ingredienteHamburguesa = new IngredienteHamburguesa
                            {
                                HamburguesaId = hamburguesaExistente.Id,
                                IngredienteId = ingredientesIncluidos[i],
                                EsExtra = false,
                                esObligatorio = ingredientesObligatorios != null && ingredientesObligatorios.Count > i ? ingredientesObligatorios[i] : false,
                                Orden = orden++
                            };
                            _context.Add(ingredienteHamburguesa);
                        }
                    }

                    // 2. Agregar ingredientes EXTRAS (opcionales con costo adicional)
                    if (ingredientesExtras != null && ingredientesExtras.Count > 0)
                    {
                        foreach (var ingredienteId in ingredientesExtras)
                        {
                            var ingredienteHamburguesa = new IngredienteHamburguesa
                            {
                                HamburguesaId = hamburguesaExistente.Id,
                                IngredienteId = ingredienteId,
                                EsExtra = true,
                                esObligatorio = false,
                                Orden = orden++
                            };
                            _context.Add(ingredienteHamburguesa);
                        }
                    }

                    _context.Update(hamburguesaExistente);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Hamburguesa actualizada exitosamente";
                    return RedirectToAction(nameof(Hamburguesas));
                }

                // Si el modelo no es válido, mostrar errores
                ViewBag.Ingredientes = await _context.Ingredientes
                    .Where(i => i.Disponible)
                    .ToListAsync();
                return View("~/Views/Admin/Hamburguesas.cshtml", hamburguesa);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al editar la hamburguesa: {ex.Message}";
                ViewBag.Ingredientes = await _context.Ingredientes
                    .Where(i => i.Disponible)
                    .ToListAsync();
                return View("~/Views/Admin/EditarHamburguesa.cshtml", hamburguesa);
            }
        }



        // GET: /admin/bebidas
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Bebidas()
        {
            ViewData["Title"] = "Gestión de Bebidas";

            var bebidas = await _context.Bebidas
                .Where(b => b.Disponible)
                .OrderBy(b => b.Nombre)
                .ToListAsync();

            return View(bebidas);
        }

        // GET: /admin/bebidas/crear
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult CrearBebida()
        {
            ViewData["Title"] = "Crear Nueva Bebida";
            return View();
        }

        // POST: /admin/bebidas/crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CrearBebida(
            Bebida bebida,
            IFormFile imagen)
        {
            try
            {
                // Remover validaciones opcionales
                ModelState.Remove("ImagenUrl");

                if (ModelState.IsValid)
                {
                    // Procesar imagen si fue cargada
                    if (imagen != null && imagen.Length > 0)
                    {
                        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                        var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();

                        if (!extensionesPermitidas.Contains(extension))
                        {
                            ModelState.AddModelError("imagen", "Solo se permiten imágenes JPG, PNG o WEBP");
                            return View(bebida);
                        }

                        // Generar nombre único para la imagen
                        var nombreArchivo = $"bebida_{Guid.NewGuid()}{extension}";
                        var rutaImagenes = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                        // Crear carpeta si no existe
                        if (!Directory.Exists(rutaImagenes))
                        {
                            Directory.CreateDirectory(rutaImagenes);
                        }

                        var rutaCompleta = Path.Combine(rutaImagenes, nombreArchivo);

                        // Guardar imagen
                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            await imagen.CopyToAsync(stream);
                        }

                        // Asignar URL relativa al modelo
                        bebida.ImagenUrl = $"/images/{nombreArchivo}";
                    }
                    else if (string.IsNullOrWhiteSpace(bebida.ImagenUrl))
                    {
                        // Imagen por defecto si no se cargó ninguna
                        bebida.ImagenUrl = "/images/bebida-default.jpg";
                    }

                    // Validar que los precios por tamaño sean coherentes
                    if (bebida.PrecioPequeno <= 0 || bebida.PrecioMediano <= 0 || bebida.PrecioGrande <= 0)
                    {
                        ModelState.AddModelError("", "Todos los precios deben ser mayores a 0");
                        return View(bebida);
                    }

                    if (bebida.PrecioPequeno >= bebida.PrecioMediano || bebida.PrecioMediano >= bebida.PrecioGrande)
                    {
                        ModelState.AddModelError("", "Los precios deben ser: Pequeño < Mediano < Grande");
                        return View(bebida);
                    }

                    // El precio base es el mediano
                    bebida.Precio = bebida.PrecioMediano;
                    bebida.Tamanio = Tamanio.Mediano;
                    bebida.Categoria = CategoriaProducto.Bebida;
                    bebida.Disponible = true;

                    _context.Add(bebida);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Bebida creada exitosamente";
                    return RedirectToAction(nameof(Bebidas));
                }

                return View(bebida);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al crear la bebida: {ex.Message}";
                return View(bebida);
            }
        }

        // GET: /admin/bebidas/editar/{id}
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> EditarBebida(int id)
        {
            ViewData["Title"] = "Editar Bebida";

            var bebida = await _context.Bebidas.FindAsync(id);
            if (bebida == null)
            {
                return NotFound();
            }

            return View(bebida);
        }

        // POST: /admin/bebidas/editar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> EditarBebida(
            int id,
            Bebida bebida,
            IFormFile? imagen) // ⬅️ IMPORTANTE: Hacer opcional con ?
        {
            if (id != bebida.Id)
            {
                return NotFound();
            }

            try
            {
                // Remover validaciones opcionales
                ModelState.Remove("ImagenUrl");
                ModelState.Remove("imagen"); // ⬅️ AGREGAR ESTO

                if (ModelState.IsValid)
                {
                    var bebidaExistente = await _context.Bebidas.FindAsync(id);
                    if (bebidaExistente == null)
                    {
                        return NotFound();
                    }

                    // Procesar imagen si fue cargada
                    if (imagen != null && imagen.Length > 0)
                    {
                        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                        var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();

                        if (!extensionesPermitidas.Contains(extension))
                        {
                            ModelState.AddModelError("imagen", "Solo se permiten imágenes JPG, PNG o WEBP");
                            return View(bebida);
                        }

                        // Eliminar imagen anterior si existe y no es la por defecto
                        if (!string.IsNullOrEmpty(bebidaExistente.ImagenUrl) &&
                            bebidaExistente.ImagenUrl != "/images/bebida-default.jpg")
                        {
                            var rutaImagenAnterior = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                bebidaExistente.ImagenUrl.TrimStart('/'));

                            if (System.IO.File.Exists(rutaImagenAnterior))
                            {
                                System.IO.File.Delete(rutaImagenAnterior);
                            }
                        }

                        // Guardar nueva imagen
                        var nombreArchivo = $"bebida_{Guid.NewGuid()}{extension}";
                        var rutaImagenes = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                        if (!Directory.Exists(rutaImagenes))
                        {
                            Directory.CreateDirectory(rutaImagenes);
                        }

                        var rutaCompleta = Path.Combine(rutaImagenes, nombreArchivo);

                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            await imagen.CopyToAsync(stream);
                        }

                        bebidaExistente.ImagenUrl = $"/images/{nombreArchivo}";
                    }
                    else if (!string.IsNullOrWhiteSpace(bebida.ImagenUrl))
                    {
                        bebidaExistente.ImagenUrl = bebida.ImagenUrl;
                    }

                    // Validar precios
                    if (bebida.PrecioPequeno <= 0 || bebida.PrecioMediano <= 0 || bebida.PrecioGrande <= 0)
                    {
                        ModelState.AddModelError("", "Todos los precios deben ser mayores a 0");
                        return View(bebida);
                    }

                    if (bebida.PrecioPequeno >= bebida.PrecioMediano || bebida.PrecioMediano >= bebida.PrecioGrande)
                    {
                        ModelState.AddModelError("", "Los precios deben ser: Pequeño < Mediano < Grande");
                        return View(bebida);
                    }

                    // Actualizar propiedades
                    bebidaExistente.Nombre = bebida.Nombre;
                    bebidaExistente.Descripcion = bebida.Descripcion;
                    bebidaExistente.PrecioPequeno = bebida.PrecioPequeno;
                    bebidaExistente.PrecioMediano = bebida.PrecioMediano;
                    bebidaExistente.PrecioGrande = bebida.PrecioGrande;
                    bebidaExistente.Precio = bebida.PrecioMediano;
                    bebidaExistente.Mililitros = bebida.Mililitros;
                    bebidaExistente.EsAlcoholica = bebida.EsAlcoholica;

                    _context.Update(bebidaExistente);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Bebida actualizada exitosamente";
                    return RedirectToAction(nameof(Bebidas));
                }

                return View(bebida);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al editar la bebida: {ex.Message}";
                return View(bebida);
            }
        }

        // POST: /admin/bebidas/eliminar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> EliminarBebida(int id)
        {
            var bebida = await _context.Bebidas.FindAsync(id);
            if (bebida == null)
            {
                return NotFound();
            }

            bebida.Disponible = false; // Soft delete
            _context.Update(bebida);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Bebida eliminada exitosamente";
            return RedirectToAction(nameof(Bebidas));
        }
    }
}