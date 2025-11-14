using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurante.Data;
using Restaurante.Models;
using Restaurante.Models.ViewModels;

namespace Restaurante.Controllers
{
    public class PersonalizacionComboController : Controller
    {
        private readonly RestauranteContext _context;

        public PersonalizacionComboController(RestauranteContext context)
        {
            _context = context;
        }

        // ✅ MOSTRAR FORMULARIO DE PERSONALIZACIÓN DEL COMBO
        public IActionResult PersonalizarCombo(int id)
        {
            var combo = _context.Combos
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .Include(c => c.BebidasDisponibles) // ✅ NUEVO: Incluir bebidas disponibles
                .ThenInclude(cb => cb.Bebida)
                .FirstOrDefault(c => c.Id == id);

            if (combo == null)
                return NotFound();

            // ✅ NUEVO: Obtener bebidas disponibles del combo
            var bebidasDisponibles = combo.BebidasDisponibles
                .Where(cb => cb.Bebida != null && cb.Bebida.Disponible)
                .Select(cb => cb.Bebida)
                .ToList();

            // ✅ NUEVO: Si no hay bebidas configuradas, usar bebidas por defecto
            if (!bebidasDisponibles.Any())
            {
                bebidasDisponibles = _context.Bebidas
                    .Where(b => b.Disponible && !b.EsAlcoholica)
                    .Take(5)
                    .ToList();
            }

            var viewModel = new PersonalizarComboViewModel
            {
                ComboId = combo.Id,
                Combo = combo,
                PrecioFinal = combo.Precio,
                BebidasDisponibles = bebidasDisponibles
            };

            // Preparar cada item del combo para personalización
            foreach (var item in combo.Items)
            {
                var itemPersonalizable = new ItemComboPersonalizable
                {
                    ItemComboId = item.Id,
                    ProductoId = item.Producto.Id,
                    ProductoNombre = item.Producto.Nombre,
                    ProductoDescripcion = item.Producto.Descripcion,
                    ProductoImagen = item.Producto.ImagenUrl,
                    Categoria = item.Producto.Categoria,
                    PrecioBase = item.Producto.Precio,
                    PrecioPersonalizado = item.Producto.Precio,
                    TamanioSeleccionado = Tamanio.Mediano // Por defecto
                };

                // ✅ NUEVO: Si es bebida, cargar recargos del combo
                if (item.Producto is Bebida bebidaItem)
                {
                    var comboBebida = combo.BebidasDisponibles
                        .FirstOrDefault(cb => cb.BebidaId == bebidaItem.Id);

                    if (comboBebida != null)
                    {
                        itemPersonalizable.RecargoPequeno = comboBebida.RecargoPequeno;
                        itemPersonalizable.RecargoMediano = comboBebida.RecargoMediano;
                        itemPersonalizable.RecargoGrande = comboBebida.RecargoGrande;
                    }
                    else
                    {
                        // ✅ NUEVO: Recargos por defecto si no están configurados
                        itemPersonalizable.RecargoPequeno = -1.00m; // Ahorro por pequeño
                        itemPersonalizable.RecargoMediano = 0.00m;  // Sin recargo
                        itemPersonalizable.RecargoGrande = 2.00m;   // Recargo por grande
                    }

                    // ✅ Establecer como bebida seleccionada por defecto
                    viewModel.BebidaSeleccionadaId = bebidaItem.Id;
                }

                // Cargar datos específicos según el tipo de producto
                switch (item.Producto)
                {
                    case Hamburguesa hamburguesa:
                        var hamburguesaConIngredientes = _context.Hamburguesas
                            .Include(h => h.Ingredientes)
                            .ThenInclude(ih => ih.Ingrediente)
                            .FirstOrDefault(h => h.Id == item.Producto.Id);

                        if (hamburguesaConIngredientes != null)
                        {
                            itemPersonalizable.IngredientesObligatorios = hamburguesaConIngredientes.Ingredientes
                                .Where(ih => !ih.EsExtra && ih.esObligatorio)
                                .Select(ih => ih.Ingrediente)
                                .ToList();

                            itemPersonalizable.IngredientesOpcionalesBase = hamburguesaConIngredientes.Ingredientes
                                .Where(ih => !ih.EsExtra && !ih.esObligatorio)
                                .Select(ih => ih.Ingrediente)
                                .ToList();

                            itemPersonalizable.IngredientesDisponibles = hamburguesaConIngredientes.Ingredientes
                                .Where(ih => ih.EsExtra)
                                .Select(ih => ih.Ingrediente)
                                .Where(i => i.Disponible)
                                .ToList();
                        }
                        break;

                    case Bebida bebida:
                        itemPersonalizable.TamanioSeleccionado = bebida.Tamanio;
                        break;

                    case Papas papas:
                        itemPersonalizable.TamanioSeleccionado = papas.Tamanio;
                        itemPersonalizable.ConSal = true;
                        itemPersonalizable.SalsaSeleccionada = papas.TipoSalsa ?? "Kétchup";
                        break;
                }

                viewModel.Items.Add(itemPersonalizable);
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ProcesarPersonalizacionCombo(PersonalizarComboViewModel model)
        {
            var combo = _context.Combos
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefault(c => c.Id == model.ComboId);

            if (combo == null)
            {
                TempData["Error"] = "Combo no encontrado";
                return RedirectToAction("Index", "Home");
            }

            decimal precioTotal = combo.Precio; // Comenzar con el precio base del combo
            var notasCombo = new List<string>();

            // ✅ NUEVO: Reemplazar la bebida original por la seleccionada
            var itemBebidaOriginal = model.Items.FirstOrDefault(i => i.EsBebida);
            if (itemBebidaOriginal != null && model.BebidaSeleccionadaId > 0)
            {
                // Cambiar el ProductoId de la bebida por la seleccionada
                itemBebidaOriginal.ProductoId = model.BebidaSeleccionadaId;

                // Actualizar nombre y datos de la bebida seleccionada
                var bebidaSeleccionada = _context.Bebidas.Find(model.BebidaSeleccionadaId);
                if (bebidaSeleccionada != null)
                {
                    itemBebidaOriginal.ProductoNombre = bebidaSeleccionada.Nombre;
                    itemBebidaOriginal.ProductoDescripcion = bebidaSeleccionada.Descripcion;
                    itemBebidaOriginal.ProductoImagen = bebidaSeleccionada.ImagenUrl;

                    // ✅ NUEVO: Actualizar recargos según la bebida seleccionada
                    var comboBebida = _context.ComboBebidas
                        .FirstOrDefault(cb => cb.ComboId == combo.Id && cb.BebidaId == model.BebidaSeleccionadaId);

                    if (comboBebida != null)
                    {
                        itemBebidaOriginal.RecargoPequeno = comboBebida.RecargoPequeno;
                        itemBebidaOriginal.RecargoMediano = comboBebida.RecargoMediano;
                        itemBebidaOriginal.RecargoGrande = comboBebida.RecargoGrande;
                    }
                }
            }

            // Procesar cada item personalizado
            foreach (var item in model.Items)
            {
                decimal extrasItem = 0;
                var notasItem = new List<string>();

                // Calcular precio según personalización
                switch (item.Categoria)
                {
                    case CategoriaProducto.Hamburguesa:
                        var hamburguesaCompleta = _context.Hamburguesas
                            .Include(h => h.Ingredientes)
                            .ThenInclude(ih => ih.Ingrediente)
                            .FirstOrDefault(h => h.Id == item.ProductoId);

                        if (hamburguesaCompleta != null)
                        {
                            // ✅ 1. Ingredientes OBLIGATORIOS (siempre incluidos)
                            var ingredientesObligatorios = hamburguesaCompleta.Ingredientes
                                .Where(ih => !ih.EsExtra && ih.esObligatorio)
                                .Select(ih => ih.Ingrediente.Nombre)
                                .ToList();

                            // ✅ 2. Ingredientes OPCIONALES que el usuario MANTIENE
                            var ingredientesOpcionalesMantenidos = new List<string>();
                            var ingredientesOpcionalesRemovidos = new List<string>();

                            var todosLosOpcionalesBase = hamburguesaCompleta.Ingredientes
                                .Where(ih => !ih.EsExtra && !ih.esObligatorio)
                                .Select(ih => ih.Ingrediente)
                                .ToList();

                            foreach (var ingredienteOpcional in todosLosOpcionalesBase)
                            {
                                if (item.IngredientesOpcionalesSeleccionados != null &&
                                    item.IngredientesOpcionalesSeleccionados.Contains(ingredienteOpcional.Id))
                                {
                                    ingredientesOpcionalesMantenidos.Add(ingredienteOpcional.Nombre);
                                }
                                else
                                {
                                    ingredientesOpcionalesRemovidos.Add(ingredienteOpcional.Nombre);
                                }
                            }

                            // Construir nota de ingredientes base
                            var ingredientesBase = new List<string>();
                            ingredientesBase.AddRange(ingredientesObligatorios);
                            ingredientesBase.AddRange(ingredientesOpcionalesMantenidos);

                            if (ingredientesBase.Any())
                            {
                                notasItem.Add($"Con: {string.Join(", ", ingredientesBase)}");
                            }

                            if (ingredientesOpcionalesRemovidos.Any())
                            {
                                notasItem.Add($"Sin: {string.Join(", ", ingredientesOpcionalesRemovidos)}");
                            }

                            // ✅ 3. Ingredientes EXTRA seleccionados
                            if (item.IngredientesExtraSeleccionados != null && item.IngredientesExtraSeleccionados.Any())
                            {
                                var extrasAgregados = new List<string>();

                                foreach (var ingredienteId in item.IngredientesExtraSeleccionados)
                                {
                                    var ingrediente = _context.Ingredientes.Find(ingredienteId);
                                    if (ingrediente != null)
                                    {
                                        extrasItem += ingrediente.PrecioAdicional;

                                        if (ingrediente.PrecioAdicional > 0)
                                        {
                                            extrasAgregados.Add($"{ingrediente.Nombre} (+S/. {ingrediente.PrecioAdicional:0.00})");
                                        }
                                        else
                                        {
                                            extrasAgregados.Add(ingrediente.Nombre);
                                        }
                                    }
                                }

                                if (extrasAgregados.Any())
                                {
                                    notasItem.Add($"Extras: {string.Join(", ", extrasAgregados)}");
                                }
                            }
                        }
                        break;

                    case CategoriaProducto.Bebida:
                        var bebida = _context.Bebidas.Find(item.ProductoId);
                        if (bebida != null)
                        {
                            // ✅ NUEVO: Usar recargos personalizados en lugar de precios fijos
                            decimal recargoTamanio = item.TamanioSeleccionado switch
                            {
                                Tamanio.Pequeno => item.RecargoPequeno,
                                Tamanio.Mediano => item.RecargoMediano,
                                Tamanio.Grande => item.RecargoGrande,
                                _ => 0
                            };

                            if (recargoTamanio > 0)
                            {
                                extrasItem += recargoTamanio;
                            }

                            notasItem.Add($"Bebida: {item.ProductoNombre}");
                            notasItem.Add($"Tamaño: {item.TamanioSeleccionado}");

                            if (recargoTamanio > 0)
                            {
                                notasItem.Add($"Recargo tamaño: +S/. {recargoTamanio:0.00}");
                            }
                        }
                        break;

                    case CategoriaProducto.Papas:
                        var papas = _context.Papas.Find(item.ProductoId);
                        if (papas != null)
                        {
                            decimal precioTamanio = papas.ObtenerPrecioPorTamanio(item.TamanioSeleccionado);
                            decimal diferenciaPrecios = precioTamanio - papas.Precio;

                            if (diferenciaPrecios > 0)
                            {
                                extrasItem += diferenciaPrecios;
                            }

                            notasItem.Add($"Tamaño: {item.TamanioSeleccionado}");
                            notasItem.Add(item.ConSal ? "Con sal" : "Sin sal");
                            notasItem.Add($"Salsa: {item.SalsaSeleccionada}");
                        }
                        break;
                }

                // Agregar notas adicionales del usuario
                if (!string.IsNullOrEmpty(item.NotasPersonalizacion))
                {
                    notasItem.Add($"Notas: {item.NotasPersonalizacion}");
                }

                // Sumar extras al precio total
                precioTotal += extrasItem;

                // Agregar notas del item al combo
                if (notasItem.Any())
                {
                    notasCombo.Add($"{item.ProductoNombre}: {string.Join(" | ", notasItem)}");
                }
            }

            // ✅ NUEVO: Agregar información de la bebida seleccionada al carrito
            var bebidaSeleccionadaInfo = _context.Bebidas.Find(model.BebidaSeleccionadaId);
            if (bebidaSeleccionadaInfo != null)
            {
                notasCombo.Add($"Bebida elegida: {bebidaSeleccionadaInfo.Nombre}");
            }

            // Agregar al carrito
            return RedirectToAction("AgregarComboPersonalizado", "Carrito", new
            {
                comboId = model.ComboId,
                precioPersonalizado = precioTotal,
                notas = string.Join("; ", notasCombo),
                bebidaSeleccionadaId = model.BebidaSeleccionadaId // ✅ NUEVO: Pasar la bebida seleccionada
            });
        }
    }
}