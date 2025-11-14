
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurante.Data;
using Restaurante.Models;
using System.IO;

namespace Restaurante.Controllers.Admin
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("admin/papas")]
    public class PapasController : Controller
    {
        private readonly RestauranteContext _context;

        public PapasController(RestauranteContext context)
        {
            _context = context;
        }

        // GET: /admin/papas
        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Gestión de Papas";

            var papas = await _context.Papas
                .Where(p => p.Disponible)
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            return View("~/Views/Admin/Papas/Index.cshtml", papas);
        }

        // GET: /admin/papas/crear
        [HttpGet("crear")]
        public IActionResult Crear()
        {
            ViewData["Title"] = "Crear Nuevas Papas";
            return View("~/Views/Admin/Papas/Crear.cshtml");
        }

        // POST: /admin/papas/crear
        [HttpPost("crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            Papas papas,
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
                            return View("~/Views/Admin/Papas/Crear.cshtml", papas);
                        }

                        // Generar nombre único para la imagen
                        var nombreArchivo = $"papas_{Guid.NewGuid()}{extension}";
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
                        papas.ImagenUrl = $"/images/{nombreArchivo}";
                    }
                    else if (string.IsNullOrWhiteSpace(papas.ImagenUrl))
                    {
                        // Imagen por defecto si no se cargó ninguna
                        papas.ImagenUrl = "/images/papas-default.jpg";
                    }

                    // Validar que los precios por tamaño sean coherentes
                    if (papas.PrecioPequeno <= 0 || papas.PrecioMediano <= 0 || papas.PrecioGrande <= 0)
                    {
                        ModelState.AddModelError("", "Todos los precios deben ser mayores a 0");
                        return View("~/Views/Admin/Papas/Editar.cshtml", papas);
                    }

                    if (papas.PrecioPequeno >= papas.PrecioMediano || papas.PrecioMediano >= papas.PrecioGrande)
                    {
                        ModelState.AddModelError("", "Los precios deben ser: Pequeño < Mediano < Grande");
                        return View("~/Views/Admin/Papas/Editar.cshtml", papas);
                    }

                    // El precio base es el mediano
                    papas.Precio = papas.PrecioMediano;
                    papas.Tamanio = Tamanio.Mediano;
                    papas.Categoria = CategoriaProducto.Papas;
                    papas.Disponible = true;

                    _context.Add(papas);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Papas creadas exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                return View("~/Views/Admin/Papas/Crear.cshtml", papas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al crear las papas: {ex.Message}";
                return View("~/Views/Admin/Papas/Editar.cshtml", papas);
            }
        }

        // GET: /admin/papas/editar/{id}
        [HttpGet("editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            ViewData["Title"] = "Editar Papas";

            var papas = await _context.Papas.FindAsync(id);
            if (papas == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Papas/Editar.cshtml", papas);
        }

        // POST: /admin/papas/editar/{id}
        [HttpPost("editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            Papas papas,
            IFormFile? imagen)
        {
            if (id != papas.Id)
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
                    var papasExistente = await _context.Papas.FindAsync(id);
                    if (papasExistente == null)
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
                            return View(papas);
                        }

                        // Eliminar imagen anterior si existe y no es la por defecto
                        if (!string.IsNullOrEmpty(papasExistente.ImagenUrl) &&
                            papasExistente.ImagenUrl != "/images/papas-default.jpg")
                        {
                            var rutaImagenAnterior = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                papasExistente.ImagenUrl.TrimStart('/'));

                            if (System.IO.File.Exists(rutaImagenAnterior))
                            {
                                System.IO.File.Delete(rutaImagenAnterior);
                            }
                        }

                        // Guardar nueva imagen
                        var nombreArchivo = $"papas_{Guid.NewGuid()}{extension}";
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

                        papasExistente.ImagenUrl = $"/images/{nombreArchivo}";
                    }
                    else if (!string.IsNullOrWhiteSpace(papas.ImagenUrl))
                    {
                        papasExistente.ImagenUrl = papas.ImagenUrl;
                    }

                    // Validar precios
                    if (papas.PrecioPequeno <= 0 || papas.PrecioMediano <= 0 || papas.PrecioGrande <= 0)
                    {
                        ModelState.AddModelError("", "Todos los precios deben ser mayores a 0");
                        return View(papas);
                    }

                    if (papas.PrecioPequeno >= papas.PrecioMediano || papas.PrecioMediano >= papas.PrecioGrande)
                    {
                        ModelState.AddModelError("", "Los precios deben ser: Pequeño < Mediano < Grande");
                        return View(papas);
                    }

                    // Actualizar propiedades
                    papasExistente.Nombre = papas.Nombre;
                    papasExistente.Descripcion = papas.Descripcion;
                    papasExistente.PrecioPequeno = papas.PrecioPequeno;
                    papasExistente.PrecioMediano = papas.PrecioMediano;
                    papasExistente.PrecioGrande = papas.PrecioGrande;
                    papasExistente.Precio = papas.PrecioMediano;
                    papasExistente.TipoSalsa = papas.TipoSalsa;

                    _context.Update(papasExistente);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Papas actualizadas exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                return View("~/Views/Admin/Papas/Editar.cshtml", papas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al editar las papas: {ex.Message}";
                return View("~/Views/Admin/Papas/Editar.cshtml", papas);
            }
        }

        // POST: /admin/papas/eliminar/{id}
        [HttpPost("eliminar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var papas = await _context.Papas.FindAsync(id);
            if (papas == null)
            {
                return NotFound();
            }

            papas.Disponible = false; // Soft delete
            _context.Update(papas);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Papas eliminadas exitosamente";
            return RedirectToAction(nameof(Index));
        }
    }
}