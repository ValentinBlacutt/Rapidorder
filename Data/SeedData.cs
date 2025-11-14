using Microsoft.EntityFrameworkCore;
using Restaurante.Models;

namespace Restaurante.Data
{
    public static class SeedData
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // ==================== INGREDIENTES ====================
            var ingredientes = new List<Ingrediente>
            {
                new Ingrediente {
                    Id = 1,
                    Nombre = "Carne de res",
                    Descripcion = "Carne 100% de res premium",
                    PrecioAdicional = 0,
                    Disponible = true
                },
                new Ingrediente {
                    Id = 2,
                    Nombre = "Pan brioche",
                    Descripcion = "Pan brioche artesanal",
                    PrecioAdicional = 0,
                    Disponible = true
                },
                new Ingrediente {
                    Id = 3,
                    Nombre = "Lechuga",
                    Descripcion = "Lechuga fresca crujiente",
                    PrecioAdicional = 0,
                    Disponible = true
                },
                new Ingrediente {
                    Id = 4,
                    Nombre = "Tomate",
                    Descripcion = "Tomate natural en rodajas",
                    PrecioAdicional = 0,
                    Disponible = true
                },
                new Ingrediente {
                    Id = 5,
                    Nombre = "Queso cheddar",
                    Descripcion = "Queso cheddar derretido",
                    PrecioAdicional = 2.00m,
                    Disponible = true
                },
                new Ingrediente {
                    Id = 6,
                    Nombre = "Tocino",
                    Descripcion = "Tocino crujiente",
                    PrecioAdicional = 3.00m,
                    Disponible = true
                },
                new Ingrediente {
                    Id = 7,
                    Nombre = "Cebolla caramelizada",
                    Descripcion = "Cebolla caramelizada en su punto",
                    PrecioAdicional = 1.50m,
                    Disponible = true
                },
                new Ingrediente {
                    Id = 8,
                    Nombre = "Pepinillos",
                    Descripcion = "Pepinillos encurtidos",
                    PrecioAdicional = 0.50m,
                    Disponible = true
                },
                new Ingrediente {
                    Id = 9,
                    Nombre = "Salsa especial",
                    Descripcion = "Salsa de la casa",
                    PrecioAdicional = 0,
                    Disponible = true
                }
            };

            // ==================== HAMBURGUESAS ====================
            var hamburguesas = new List<Hamburguesa>
            {
                new Hamburguesa {
                    Id = 1,
                    Nombre = "Clásica",
                    Descripcion = "Hamburguesa clásica con carne, lechuga y tomate",
                    Precio = 12.00m,
                    ImagenUrl = "/images/hamburguesa-clasica.jpg",
                    Disponible = true,
                    Categoria = CategoriaProducto.Hamburguesa,
                    EsVegetariana = false
                },
                new Hamburguesa {
                    Id = 2,
                    Nombre = "Doble Queso",
                    Descripcion = "Hamburguesa con doble carne y queso cheddar",
                    Precio = 18.00m,
                    ImagenUrl = "/images/hamburguesa-doble-queso.jpg",
                    Disponible = true,
                    Categoria = CategoriaProducto.Hamburguesa,
                    EsVegetariana = false
                }
            };

            // ==================== PAPAS ====================
            var papas = new List<Papas>
            {
                new Papas {
                    Id = 3,
                    Nombre = "Papas Fritas Clásicas",
                    Descripcion = "Papas fritas crujientes",
                    Precio = 5.00m, // Precio base (mediano)
                    PrecioPequeno = 3.50m,
                    PrecioMediano = 5.00m,
                    PrecioGrande = 6.50m,
                    ImagenUrl = "/images/papas-fritas.jpg",
                    Tamanio = Tamanio.Mediano,
                    TipoSalsa = "Kétchup",
                    Disponible = true,
                    Categoria = CategoriaProducto.Papas
                },
                new Papas {
                    Id = 8,
                    Nombre = "Papas con Cheese Bacon",
                    Descripcion = "Papas con queso cheddar y tocino",
                    Precio = 8.00m, // Precio base (mediano)
                    PrecioPequeno = 6.00m,
                    PrecioMediano = 8.00m,
                    PrecioGrande = 10.00m,
                    ImagenUrl = "/images/papas-chesse-bacon.jpg",
                    Tamanio = Tamanio.Mediano,
                    TipoSalsa = "Queso cheddar",
                    Disponible = true,
                    Categoria = CategoriaProducto.Papas
                }
            };

            // ==================== BEBIDAS ====================
            var bebidas = new List<Bebida>
            {
                new Bebida {
                    Id = 4,
                    Nombre = "Coca Cola",
                    Descripcion = "Refresco de cola",
                    Precio = 3.50m, // ✅ DEBE SER IGUAL a PrecioMediano
                    PrecioPequeno = 2.50m,
                    PrecioMediano = 3.50m, // ✅ IGUAL que Precio
                    PrecioGrande = 4.50m,
                    ImagenUrl = "/images/coca-cola.jpg",
                    Tamanio = Tamanio.Mediano,
                    EsAlcoholica = false,
                    Mililitros = 500,
                    Disponible = true,
                    Categoria = CategoriaProducto.Bebida
                },
                new Bebida {
                    Id = 9,
                    Nombre = "Jugo de Naranja",
                    Descripcion = "Jugo natural de naranja",
                    Precio = 4.00m, // ✅ DEBE SER IGUAL a PrecioMediano
                    PrecioPequeno = 3.00m,
                    PrecioMediano = 4.00m, // ✅ IGUAL que Precio
                    PrecioGrande = 5.50m,
                    ImagenUrl = "/images/jugo-naranja.jpg",
                    Tamanio = Tamanio.Mediano,
                    EsAlcoholica = false,
                    Mililitros = 400,
                    Disponible = true,
                    Categoria = CategoriaProducto.Bebida
                },
                new Bebida {
                    Id = 10,
                    Nombre = "Cerveza Artesanal",
                    Descripcion = "Cerveza artesanal de la casa",
                    Precio = 8.00m, // ✅ DEBE SER IGUAL a PrecioMediano
                    PrecioPequeno = 6.00m,
                    PrecioMediano = 8.00m, // ✅ IGUAL que Precio
                    PrecioGrande = 10.00m,
                    ImagenUrl = "/images/cerveza.jpg",
                    Tamanio = Tamanio.Mediano,
                    EsAlcoholica = true,
                    Mililitros = 500,
                    Disponible = true,
                    Categoria = CategoriaProducto.Bebida
                },
                // ✅ NUEVO: Más bebidas para los combos
                new Bebida {
                    Id = 11,
                    Nombre = "Pepsi",
                    Descripcion = "Refresco de cola",
                    Precio = 3.00m,
                    PrecioPequeno = 2.00m,
                    PrecioMediano = 3.00m,
                    PrecioGrande = 4.00m,
                    ImagenUrl = "/images/pepsi.jpg",
                    Tamanio = Tamanio.Mediano,
                    EsAlcoholica = false,
                    Mililitros = 500,
                    Disponible = true,
                    Categoria = CategoriaProducto.Bebida
                },
                new Bebida {
                    Id = 12,
                    Nombre = "Fanta",
                    Descripcion = "Refresco de naranja",
                    Precio = 3.00m,
                    PrecioPequeno = 2.00m,
                    PrecioMediano = 3.00m,
                    PrecioGrande = 4.00m,
                    ImagenUrl = "/images/fanta.jpg",
                    Tamanio = Tamanio.Mediano,
                    EsAlcoholica = false,
                    Mililitros = 500,
                    Disponible = true,
                    Categoria = CategoriaProducto.Bebida
                },
                new Bebida {
                    Id = 13,
                    Nombre = "Agua Mineral",
                    Descripcion = "Agua sin gas",
                    Precio = 2.50m,
                    PrecioPequeno = 1.50m,
                    PrecioMediano = 2.50m,
                    PrecioGrande = 3.50m,
                    ImagenUrl = "/images/agua.jpg",
                    Tamanio = Tamanio.Mediano,
                    EsAlcoholica = false,
                    Mililitros = 500,
                    Disponible = true,
                    Categoria = CategoriaProducto.Bebida
                }
            };

            // ==================== COMBOS ====================
            var combos = new List<Combo>
            {
                new Combo {
                    Id = 1,
                    Nombre = "Combo Clásico",
                    Descripcion = "Hamburguesa Clásica + Papas + Bebida",
                    Precio = 18.00m,
                    ImagenUrl = "/images/combo-clasico.jpg",
                    Disponible = true
                },
                new Combo {
                    Id = 2,
                    Nombre = "Combo Doble Queso",
                    Descripcion = "Hamburguesa Doble Queso + Papas + Bebida",
                    Precio = 25.00m,
                    ImagenUrl = "/images/combo-doble-queso.jpg",
                    Disponible = true
                }
            };

            // ==================== ✅ NUEVO: COMBO BEBIDAS ====================
            var comboBebidas = new List<ComboBebida>
            {
                // Combo Clásico (Id=1) - Bebidas disponibles
                new ComboBebida {
                    Id = 1,
                    ComboId = 1,
                    BebidaId = 4, // Coca Cola
                    RecargoPequeno = -1.00m, // Ahorro por tamaño pequeño
                    RecargoMediano = 0.00m,  // Sin recargo (tamaño base)
                    RecargoGrande = 1.00m    // Recargo por tamaño grande
                },
                new ComboBebida {
                    Id = 2,
                    ComboId = 1,
                    BebidaId = 9, // Jugo de Naranja
                    RecargoPequeno = -0.50m,
                    RecargoMediano = 0.00m,
                    RecargoGrande = 1.50m
                },
                new ComboBebida {
                    Id = 3,
                    ComboId = 1,
                    BebidaId = 11, // Pepsi
                    RecargoPequeno = -1.00m,
                    RecargoMediano = 0.00m,
                    RecargoGrande = 1.00m
                },
                new ComboBebida {
                    Id = 4,
                    ComboId = 1,
                    BebidaId = 12, // Fanta
                    RecargoPequeno = -1.00m,
                    RecargoMediano = 0.00m,
                    RecargoGrande = 1.00m
                },
                new ComboBebida {
                    Id = 5,
                    ComboId = 1,
                    BebidaId = 13, // Agua Mineral
                    RecargoPequeno = -0.50m,
                    RecargoMediano = 0.00m,
                    RecargoGrande = 0.50m
                },

                // Combo Doble Queso (Id=2) - Bebidas disponibles
                new ComboBebida {
                    Id = 6,
                    ComboId = 2,
                    BebidaId = 4, // Coca Cola
                    RecargoPequeno = -1.50m,
                    RecargoMediano = 0.00m,
                    RecargoGrande = 1.50m
                },
                new ComboBebida {
                    Id = 7,
                    ComboId = 2,
                    BebidaId = 9, // Jugo de Naranja
                    RecargoPequeno = -1.00m,
                    RecargoMediano = 0.00m,
                    RecargoGrande = 2.00m
                },
                new ComboBebida {
                    Id = 8,
                    ComboId = 2,
                    BebidaId = 10, // Cerveza Artesanal
                    RecargoPequeno = -2.00m,
                    RecargoMediano = 0.00m,
                    RecargoGrande = 2.00m
                },
                new ComboBebida {
                    Id = 9,
                    ComboId = 2,
                    BebidaId = 11, // Pepsi
                    RecargoPequeno = -1.50m,
                    RecargoMediano = 0.00m,
                    RecargoGrande = 1.50m
                },
                new ComboBebida {
                    Id = 10,
                    ComboId = 2,
                    BebidaId = 13, // Agua Mineral
                    RecargoPequeno = -1.00m,
                    RecargoMediano = 0.00m,
                    RecargoGrande = 1.00m
                }
            };

            // ==================== RELACIONES INGREDIENTE-HAMBURGUESA ====================
            var ingredienteHamburguesas = new List<IngredienteHamburguesa>
            {
                // === HAMBURGUESA CLÁSICA (Id=1) ===
                // Ingredientes BASE (incluidos, no se pueden quitar)
                new IngredienteHamburguesa {
                    Id = 1,
                    HamburguesaId = 1,
                    IngredienteId = 1, // Carne
                    EsExtra = false,
                    esObligatorio = true
                },
                new IngredienteHamburguesa {
                    Id = 2,
                    HamburguesaId = 1,
                    IngredienteId = 2, // Pan
                    EsExtra = false,
                    esObligatorio = true
                },
                new IngredienteHamburguesa {
                    Id = 3,
                    HamburguesaId = 1,
                    IngredienteId = 3, // Lechuga
                    EsExtra = false,
                    esObligatorio = false
                },
                new IngredienteHamburguesa {
                    Id = 4,
                    HamburguesaId = 1,
                    IngredienteId = 4, // Tomate
                    EsExtra = false,
                    esObligatorio = false
                },
                
                // Ingredientes EXTRA (opcionales, agregan costo)
                new IngredienteHamburguesa {
                    Id = 5,
                    HamburguesaId = 1,
                    IngredienteId = 5, // Queso cheddar
                    EsExtra = true,
                    esObligatorio = false
                },
                new IngredienteHamburguesa {
                    Id = 6,
                    HamburguesaId = 1,
                    IngredienteId = 6, // Tocino
                    EsExtra = true,
                    esObligatorio = false
                },
                new IngredienteHamburguesa {
                    Id = 7,
                    HamburguesaId = 1,
                    IngredienteId = 7, // Cebolla caramelizada
                    EsExtra = true,
                    esObligatorio = false
                },
                new IngredienteHamburguesa {
                    Id = 8,
                    HamburguesaId = 1,
                    IngredienteId = 8, // Pepinillos
                    EsExtra = true,
                    esObligatorio = false
                },

                // === HAMBURGUESA DOBLE QUESO (Id=2) ===
                // Ingredientes BASE
                new IngredienteHamburguesa {
                    Id = 9,
                    HamburguesaId = 2,
                    IngredienteId = 1, // Carne (doble)
                    EsExtra = false,
                    esObligatorio = true
                },
                new IngredienteHamburguesa {
                    Id = 10,
                    HamburguesaId = 2,
                    IngredienteId = 2, // Pan
                    EsExtra = false,
                    esObligatorio = true
                },
                new IngredienteHamburguesa {
                    Id = 11,
                    HamburguesaId = 2,
                    IngredienteId = 5, // Queso cheddar (incluido)
                    EsExtra = false,
                    esObligatorio = false
                },
                new IngredienteHamburguesa {
                    Id = 12,
                    HamburguesaId = 2,
                    IngredienteId = 3, // Lechuga
                    EsExtra = false,
                    esObligatorio = false
                },
                new IngredienteHamburguesa {
                    Id = 13,
                    HamburguesaId = 2,
                    IngredienteId = 9, // Salsa especial
                    EsExtra = false,
                    esObligatorio = false
                },
                
                // Ingredientes EXTRA
                new IngredienteHamburguesa {
                    Id = 14,
                    HamburguesaId = 2,
                    IngredienteId = 6, // Tocino
                    EsExtra = true,
                    esObligatorio = false
                },
                new IngredienteHamburguesa {
                    Id = 15,
                    HamburguesaId = 2,
                    IngredienteId = 7, // Cebolla caramelizada
                    EsExtra = true,
                    esObligatorio = false
                },
                new IngredienteHamburguesa {
                    Id = 16,
                    HamburguesaId = 2,
                    IngredienteId = 4, // Tomate
                    EsExtra = true,
                    esObligatorio = false
                },
                new IngredienteHamburguesa {
                    Id = 17,
                    HamburguesaId = 2,
                    IngredienteId = 8, // Pepinillos
                    EsExtra = true,
                    esObligatorio = false
                }
            };

            // ==================== ITEMS DE COMBOS ====================
            var itemCombos = new List<ItemCombo>
            {
                new ItemCombo { Id = 1, ComboId = 1, ProductoId = 1, Cantidad = 1 },
                new ItemCombo { Id = 2, ComboId = 1, ProductoId = 3, Cantidad = 1 },
                new ItemCombo { Id = 3, ComboId = 1, ProductoId = 4, Cantidad = 1 },

                new ItemCombo { Id = 4, ComboId = 2, ProductoId = 2, Cantidad = 1 },
                new ItemCombo { Id = 5, ComboId = 2, ProductoId = 3, Cantidad = 1 },
                new ItemCombo { Id = 6, ComboId = 2, ProductoId = 4, Cantidad = 1 }
            };

            // ==================== USUARIOS ADMIN ====================
            var usuariosAdmin = new List<UsuarioAdmin>
            {
                new UsuarioAdmin
                {
                    Id = 1,
                    Email = "admin@restaurante.com",
                    PasswordHash = "$2a$11$HcrEosJ8vlF5n/y.omjmzOJB0N8EioeE/GV1IE3BxUOyH216dEps2", // ✅ String fijo
                    Nombre = "Administrador Principal",
                    Rol = RolUsuario.SuperAdmin,
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), // ✅ Fecha fija
                    UltimoAcceso = null
                },
                new UsuarioAdmin
                {
                    Id = 2,
                    Email = "cocina@restaurante.com",
                    PasswordHash = "$2a$11$s/OkGzemh4.Gp4epaONeLOsY.zDtgIVJT3TAzkTintVwlUn29gO4i", // ✅ String fijo
                    Nombre = "Usuario Cocina",
                    Rol = RolUsuario.Cocina,
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), // ✅ Fecha fija
                    UltimoAcceso = null
                }
            };

            // ==================== APLICAR DATOS ====================
            modelBuilder.Entity<Ingrediente>().HasData(ingredientes);
            modelBuilder.Entity<Hamburguesa>().HasData(hamburguesas);
            modelBuilder.Entity<Papas>().HasData(papas);
            modelBuilder.Entity<Bebida>().HasData(bebidas);
            modelBuilder.Entity<Combo>().HasData(combos);
            modelBuilder.Entity<ItemCombo>().HasData(itemCombos);
            modelBuilder.Entity<ComboBebida>().HasData(comboBebidas); // ✅ NUEVO
            modelBuilder.Entity<IngredienteHamburguesa>().HasData(ingredienteHamburguesas);
            modelBuilder.Entity<UsuarioAdmin>().HasData(usuariosAdmin);
        }
    }
}