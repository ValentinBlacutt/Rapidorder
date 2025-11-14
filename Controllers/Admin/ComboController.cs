// Controllers/Admin/CombosController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurante.Data;
using Restaurante.Models;
using Restaurante.Models.ViewModels;

namespace Restaurante.Controllers.Admin
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("admin/combos")]
    public class AdminCombosController : Controller
    {
        private readonly RestauranteContext _context;

        public AdminCombosController(RestauranteContext context)
        {
            _context = context;
        }

        // ✅ MOSTRAR LISTA DE COMBOS
        public async Task<IActionResult> Index()
        {
            var combos = await _context.Combos
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .Include(c => c.BebidasDisponibles)
                .ThenInclude(cb => cb.Bebida)
                .Where(c => c.Disponible)
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            return View("~/Views/Admin/Combos/Index.cshtml", combos);
        }

        // ✅ MOSTRAR FORMULARIO CREAR COMBO
        [HttpGet("crear")]
        public async Task<IActionResult> Crear()
        {
            var viewModel = await CargarViewModel();
            return View("~/Views/Admin/Combos/Crear.cshtml", viewModel);
        }

        // 3. ACTUALIZA el método Crear para procesar las cantidades

        [HttpPost("crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearEditarComboViewModel model, IFormFile? imagen)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = await CargarViewModel();
                viewModel.Nombre = model.Nombre;
                viewModel.Descripcion = model.Descripcion;
                viewModel.Precio = model.Precio;
                viewModel.Disponible = model.Disponible;
                viewModel.ProductosDelCombo = model.ProductosDelCombo ?? new List<ItemComboViewModel>();
                viewModel.BebidasDisponibles = model.BebidasDisponibles ?? new List<BebidaComboViewModel>();
                return View("~/Views/Admin/Combos/Crear.cshtml", viewModel);
            }

            try
            {
                string imagenUrl = "/images/combo-default.jpg";

                if (imagen != null && imagen.Length > 0)
                {
                    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();

                    if (!extensionesPermitidas.Contains(extension))
                    {
                        ModelState.AddModelError("", "Solo se permiten imágenes JPG, PNG o WEBP");
                        var viewModel = await CargarViewModel();
                        viewModel.Nombre = model.Nombre;
                        viewModel.Descripcion = model.Descripcion;
                        viewModel.Precio = model.Precio;
                        viewModel.ProductosDelCombo = model.ProductosDelCombo ?? new List<ItemComboViewModel>();
                        viewModel.BebidasDisponibles = model.BebidasDisponibles ?? new List<BebidaComboViewModel>();
                        return View("~/Views/Admin/Combos/Crear.cshtml", viewModel);
                    }

                    var nombreArchivo = $"combo_{Guid.NewGuid()}{extension}";
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

                    imagenUrl = $"/images/{nombreArchivo}";
                }

                var combo = new Combo
                {
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion ?? string.Empty,
                    Precio = model.Precio,
                    ImagenUrl = imagenUrl,
                    Disponible = model.Disponible,
                    Items = new List<ItemCombo>(),
                    BebidasDisponibles = new List<ComboBebida>()
                };

                // ✅ CAMBIO: Agregar productos con cantidades
                if (model.ProductosDelCombo != null && model.ProductosDelCombo.Any())
                {
                    foreach (var item in model.ProductosDelCombo.Where(p => p.Cantidad > 0))
                    {
                        combo.Items.Add(new ItemCombo
                        {
                            ProductoId = item.ProductoId,
                            Cantidad = item.Cantidad
                        });
                    }
                }

                // Agregar bebidas disponibles al combo
                if (model.BebidasDisponibles != null && model.BebidasDisponibles.Any())
                {
                    foreach (var bebidaVm in model.BebidasDisponibles.Where(b => b.Seleccionada))
                    {
                        combo.BebidasDisponibles.Add(new ComboBebida
                        {
                            BebidaId = bebidaVm.BebidaId,
                            RecargoPequeno = bebidaVm.RecargoPequeno,
                            RecargoMediano = bebidaVm.RecargoMediano,
                            RecargoGrande = bebidaVm.RecargoGrande
                        });
                    }
                }

                _context.Combos.Add(combo);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Combo creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al crear el combo: {ex.Message}";
                var viewModel = await CargarViewModel();
                viewModel.Nombre = model.Nombre;
                viewModel.Descripcion = model.Descripcion;
                viewModel.Precio = model.Precio;
                viewModel.ProductosDelCombo = model.ProductosDelCombo ?? new List<ItemComboViewModel>();
                viewModel.BebidasDisponibles = model.BebidasDisponibles ?? new List<BebidaComboViewModel>();
                return View("~/Views/Admin/Combos/Crear.cshtml", viewModel);
            }
        }

        // ✅ MOSTRAR FORMULARIO EDITAR COMBO
        [HttpGet("editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            var combo = await _context.Combos
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .Include(c => c.BebidasDisponibles)
                .ThenInclude(cb => cb.Bebida)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (combo == null)
            {
                return NotFound();
            }

            var viewModel = await CargarViewModel(combo);
            return View("~/Views/Admin/Combos/Editar.cshtml", viewModel);
        }

        [HttpPost("editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, CrearEditarComboViewModel model, IFormFile? imagen)
        {
            Console.WriteLine($"=== INICIANDO EDICIÓN COMBO ID: {id} ===");
            Console.WriteLine($"Nombre: {model.Nombre}");
            Console.WriteLine($"Precio: {model.Precio}");

            if (model.ProductosDelCombo != null)
            {
                Console.WriteLine($"Productos recibidos: {model.ProductosDelCombo.Count}");
                foreach (var producto in model.ProductosDelCombo)
                {
                    Console.WriteLine($"  - ProductoId: {producto.ProductoId}, Nombre: {producto.Nombre}, Cantidad: {producto.Cantidad}");
                }
            }
            else
            {
                Console.WriteLine("ProductosDelCombo es NULL");
            }

            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState no es válido:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"  - {error.ErrorMessage}");
                }

                var viewModel = await CargarViewModel();
                viewModel.Id = model.Id;
                viewModel.Nombre = model.Nombre;
                viewModel.Descripcion = model.Descripcion;
                viewModel.Precio = model.Precio;
                viewModel.Disponible = model.Disponible;
                viewModel.ProductosDelCombo = model.ProductosDelCombo ?? new List<ItemComboViewModel>();
                viewModel.BebidasDisponibles = model.BebidasDisponibles ?? new List<BebidaComboViewModel>();
                return View("~/Views/Admin/Combos/Editar.cshtml", viewModel);
            }

            try
            {
                var combo = await _context.Combos
                    .Include(c => c.Items)
                    .Include(c => c.BebidasDisponibles)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (combo == null)
                {
                    return NotFound();
                }

                Console.WriteLine($"Combo encontrado: {combo.Nombre}");
                Console.WriteLine($"Items antes de limpiar: {combo.Items.Count}");

                // Procesar imagen si hay una nueva
                if (imagen != null && imagen.Length > 0)
                {
                    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();

                    if (!extensionesPermitidas.Contains(extension))
                    {
                        ModelState.AddModelError("", "Solo se permiten imágenes JPG, PNG o WEBP");
                        var viewModel = await CargarViewModel();
                        viewModel.Id = model.Id;
                        viewModel.Nombre = model.Nombre;
                        viewModel.Descripcion = model.Descripcion;
                        viewModel.Precio = model.Precio;
                        viewModel.ProductosDelCombo = model.ProductosDelCombo ?? new List<ItemComboViewModel>();
                        viewModel.BebidasDisponibles = model.BebidasDisponibles ?? new List<BebidaComboViewModel>();
                        return View("~/Views/Admin/Combos/Editar.cshtml", viewModel);
                    }

                    var nombreArchivo = $"combo_{Guid.NewGuid()}{extension}";
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

                    combo.ImagenUrl = $"/images/{nombreArchivo}";
                }

                // Actualizar propiedades básicas
                combo.Nombre = model.Nombre;
                combo.Descripcion = model.Descripcion ?? string.Empty;
                combo.Precio = model.Precio;
                combo.Disponible = model.Disponible;

                // ✅ LIMPIAR Y RECREAR ITEMS EXISTENTES
                _context.ItemCombos.RemoveRange(combo.Items);
                await _context.SaveChangesAsync(); // Guardar cambios inmediatamente

                combo.Items.Clear();

                // ✅ AGREGAR NUEVOS ITEMS CON CANTIDADES
                if (model.ProductosDelCombo != null && model.ProductosDelCombo.Any(p => p.Cantidad > 0))
                {
                    foreach (var item in model.ProductosDelCombo.Where(p => p.Cantidad > 0))
                    {
                        var nuevoItem = new ItemCombo
                        {
                            ComboId = combo.Id,
                            ProductoId = item.ProductoId,
                            Cantidad = item.Cantidad
                        };
                        combo.Items.Add(nuevoItem);
                        Console.WriteLine($"Agregando item: ProductoId={item.ProductoId}, Cantidad={item.Cantidad}");
                    }
                }

                // ✅ LIMPIAR Y RECREAR BEBIDAS DISPONIBLES
                _context.ComboBebidas.RemoveRange(combo.BebidasDisponibles);
                combo.BebidasDisponibles.Clear();

                if (model.BebidasDisponibles != null && model.BebidasDisponibles.Any(b => b.Seleccionada))
                {
                    foreach (var bebidaVm in model.BebidasDisponibles.Where(b => b.Seleccionada))
                    {
                        combo.BebidasDisponibles.Add(new ComboBebida
                        {
                            ComboId = combo.Id,
                            BebidaId = bebidaVm.BebidaId,
                            RecargoPequeno = bebidaVm.RecargoPequeno,
                            RecargoMediano = bebidaVm.RecargoMediano,
                            RecargoGrande = bebidaVm.RecargoGrande
                        });
                    }
                }

                _context.Combos.Update(combo);
                await _context.SaveChangesAsync();

                Console.WriteLine("=== EDICIÓN COMPLETADA EXITOSAMENTE ===");
                TempData["Success"] = "Combo actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR: {ex.Message} ===");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                TempData["Error"] = $"Error al editar el combo: {ex.Message}";
                var viewModel = await CargarViewModel();
                viewModel.Id = model.Id;
                viewModel.Nombre = model.Nombre;
                viewModel.Descripcion = model.Descripcion;
                viewModel.Precio = model.Precio;
                viewModel.ProductosDelCombo = model.ProductosDelCombo ?? new List<ItemComboViewModel>();
                viewModel.BebidasDisponibles = model.BebidasDisponibles ?? new List<BebidaComboViewModel>();
                return View("~/Views/Admin/Combos/Editar.cshtml", viewModel);
            }
        }

        // ✅ ELIMINAR COMBO (SOFT DELETE)
        [HttpPost("eliminar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo == null)
            {
                return NotFound();
            }

            combo.Disponible = false;
            _context.Combos.Update(combo);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Combo eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        private async Task<CrearEditarComboViewModel> CargarViewModel(Combo? comboExistente = null)
        {
            var hamburguesas = await _context.Hamburguesas
                .Where(h => h.Disponible)
                .OrderBy(h => h.Nombre)
                .ToListAsync();

            var papas = await _context.Papas
                .Where(p => p.Disponible)
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            var bebidas = await _context.Bebidas
                .Where(b => b.Disponible && !b.EsAlcoholica)
                .OrderBy(b => b.Nombre)
                .ToListAsync();

            var viewModel = new CrearEditarComboViewModel
            {
                HamburguesasDisponibles = hamburguesas,
                PapasDisponibles = papas,
                TodasLasBebidas = bebidas,
                BebidasDisponibles = new List<BebidaComboViewModel>(),
                ProductosDelCombo = new List<ItemComboViewModel>()
            };

            // Si estamos editando, cargar datos existentes
            if (comboExistente != null)
            {
                viewModel.Id = comboExistente.Id;
                viewModel.Nombre = comboExistente.Nombre;
                viewModel.Descripcion = comboExistente.Descripcion;
                viewModel.Precio = comboExistente.Precio;
                viewModel.ImagenUrl = comboExistente.ImagenUrl;
                viewModel.Disponible = comboExistente.Disponible;

                // ✅ CARGAR PRODUCTOS CON CANTIDADES EXISTENTES
                viewModel.ProductosDelCombo = comboExistente.Items.Select(i => new ItemComboViewModel
                {
                    ProductoId = i.ProductoId,
                    Nombre = i.Producto?.Nombre ?? "Producto no encontrado",
                    Cantidad = i.Cantidad
                }).ToList();

                // Cargar bebidas con sus configuraciones
                foreach (var bebida in bebidas)
                {
                    var comboBebida = comboExistente.BebidasDisponibles
                        .FirstOrDefault(cb => cb.BebidaId == bebida.Id);

                    viewModel.BebidasDisponibles.Add(new BebidaComboViewModel
                    {
                        BebidaId = bebida.Id,
                        Nombre = bebida.Nombre,
                        Seleccionada = comboBebida != null,
                        RecargoPequeno = comboBebida?.RecargoPequeno ?? -1.00m,
                        RecargoMediano = comboBebida?.RecargoMediano ?? 0.00m,
                        RecargoGrande = comboBebida?.RecargoGrande ?? 2.00m
                    });
                }
            }
            else
            {
                // Si es creación nueva, inicializar todas las bebidas
                foreach (var bebida in bebidas)
                {
                    viewModel.BebidasDisponibles.Add(new BebidaComboViewModel
                    {
                        BebidaId = bebida.Id,
                        Nombre = bebida.Nombre,
                        Seleccionada = false,
                        RecargoPequeno = -1.00m,
                        RecargoMediano = 0.00m,
                        RecargoGrande = 2.00m
                    });
                }
            }

            return viewModel;
        }
    }
}