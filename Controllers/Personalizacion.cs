using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurante.Data;
using Restaurante.Models.ViewModels;

public class PersonalizacionController : Controller
{
    private readonly RestauranteContext _context;

    public PersonalizacionController(RestauranteContext context)
    {
        _context = context;
    }

    // GET: Personalizar Hamburguesa
    public IActionResult PersonalizarHamburguesa(int id)
    {
        var hamburguesa = _context.Hamburguesas
            .Include(h => h.Ingredientes)
            .ThenInclude(ih => ih.Ingrediente)
            .FirstOrDefault(h => h.Id == id);

        if (hamburguesa == null)
            return NotFound();

        // ✅ 1. Ingredientes OBLIGATORIOS (esObligatorio = true, EsExtra = false)
        var ingredientesObligatorios = hamburguesa.Ingredientes
            .Where(ih => !ih.EsExtra && ih.esObligatorio)
            .Select(ih => ih.Ingrediente)
            .ToList();

        // ✅ 2. Ingredientes base OPCIONALES (esObligatorio = false, EsExtra = false)
        var ingredientesOpcionalesBase = hamburguesa.Ingredientes
            .Where(ih => !ih.EsExtra && !ih.esObligatorio)
            .Select(ih => ih.Ingrediente)
            .ToList();

        // ✅ 3. Ingredientes EXTRAS disponibles (EsExtra = true)
        var ingredientesExtraIds = hamburguesa.Ingredientes
            .Where(ih => ih.EsExtra)
            .Select(ih => ih.IngredienteId)
            .ToList();

        var ingredientesDisponibles = _context.Ingredientes
            .Where(i => i.Disponible && ingredientesExtraIds.Contains(i.Id))
            .ToList();

        var viewModel = new PersonalizarHamburguesaViewModel
        {
            HamburguesaId = hamburguesa.Id,
            Hamburguesa = hamburguesa,
            IngredientesObligatorios = ingredientesObligatorios,
            IngredientesOpcionalesBase = ingredientesOpcionalesBase,
            IngredientesDisponibles = ingredientesDisponibles,
            IngredientesExtra = new List<int>(),
            NotasAdicionales = ""
        };

        return View(viewModel);
    }

    // POST: Procesar Personalización
    [HttpPost]
    public IActionResult ProcesarPersonalizacionHamburguesa(
        int hamburguesaId,
        string ingredientesExtra,
        string notasAdicionales)
    {
        var hamburguesa = _context.Hamburguesas
            .Include(h => h.Ingredientes)
            .ThenInclude(ih => ih.Ingrediente)
            .FirstOrDefault(h => h.Id == hamburguesaId);

        if (hamburguesa == null)
        {
            TempData["Error"] = "Hamburguesa no encontrada";
            return RedirectToAction("Index", "Home");
        }

        // Calcular precio total comenzando con el precio base
        decimal precioTotal = hamburguesa.Precio;
        var notas = new List<string>();

        // ✅ Obtener ingredientes OBLIGATORIOS (siempre incluidos)
        var ingredientesObligatorios = hamburguesa.Ingredientes
            .Where(ih => !ih.EsExtra && ih.esObligatorio)
            .Select(ih => ih.Ingrediente.Nombre)
            .ToList();

        // Agregar ingredientes obligatorios a las notas
        if (ingredientesObligatorios.Any())
        {
            notas.Add($"Base obligatoria: {string.Join(", ", ingredientesObligatorios)}");
        }

        // Procesar ingredientes EXTRA si los hay
        if (!string.IsNullOrEmpty(ingredientesExtra))
        {
            var ingredienteIds = ingredientesExtra.Split(',')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(id => int.Parse(id.Trim()))
                .ToList();

            foreach (var ingredienteId in ingredienteIds)
            {
                var ingrediente = _context.Ingredientes.Find(ingredienteId);
                if (ingrediente != null)
                {
                    precioTotal += ingrediente.PrecioAdicional;

                    if (ingrediente.PrecioAdicional > 0)
                    {
                        notas.Add($"Extra: {ingrediente.Nombre} (+S/. {ingrediente.PrecioAdicional:0.00})");
                    }
                    else
                    {
                        notas.Add($"Extra: {ingrediente.Nombre} (Gratis)");
                    }
                }
            }
        }

        // Agregar notas adicionales del usuario
        if (!string.IsNullOrEmpty(notasAdicionales))
        {
            notas.Add($"Notas: {notasAdicionales.Trim()}");
        }

        // Redirigir al método del CarritoController
        return RedirectToAction("AgregarProductoPersonalizado", "Carrito", new
        {
            productoId = hamburguesaId,
            precioPersonalizado = precioTotal,
            notas = string.Join("; ", notas)
        });
    }
}