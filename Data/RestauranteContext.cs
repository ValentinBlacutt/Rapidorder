using Microsoft.EntityFrameworkCore;
using Restaurante.Models;

namespace Restaurante.Data
{
    public class RestauranteContext : DbContext
    {
        public RestauranteContext(DbContextOptions<RestauranteContext> options) : base(options) { }

        // DbSets para las entidades principales
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Hamburguesa> Hamburguesas { get; set; }
        public DbSet<Papas> Papas { get; set; }
        public DbSet<Bebida> Bebidas { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ItemCombo> ItemCombos { get; set; }
        public DbSet<UsuarioAdmin> UsuariosAdmin { get; set; }

        // ✅ COMENTAR o ELIMINAR estos DbSets si usas Session
        // public DbSet<Carrito> Carritos { get; set; }
        // public DbSet<ItemCarrito> ItemCarritos { get; set; }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItemPedidos { get; set; }
        public DbSet<Ingrediente> Ingredientes { get; set; }
        public DbSet<IngredienteHamburguesa> IngredienteHamburguesas { get; set; }

        // ✅ NUEVO: DbSet para ComboBebida
        public DbSet<ComboBebida> ComboBebidas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de precisión para campos decimales
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Combo>()
                .Property(c => c.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Ingrediente>()
                .Property(i => i.PrecioAdicional)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ItemPedido>()
                .Property(ip => ip.PrecioUnitario)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ItemPedido>()
                .Property(ip => ip.Subtotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Subtotal)
                .HasPrecision(18, 2);

            // ✅ COMENTAR estas configuraciones si usas Session
            /*
            modelBuilder.Entity<ItemCarrito>()
                .Property(ic => ic.PrecioUnitario)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ItemCarrito>()
                .Property(ic => ic.Subtotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Carrito>()
                .Property(c => c.Total)
                .HasPrecision(18, 2);
            */

            // Configuración para Bebida
            modelBuilder.Entity<Bebida>()
                .Property(b => b.PrecioPequeno)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Bebida>()
                .Property(b => b.PrecioMediano)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Bebida>()
                .Property(b => b.PrecioGrande)
                .HasPrecision(18, 2);

            // Configuración para Papas
            modelBuilder.Entity<Papas>()
                .Property(p => p.PrecioPequeno)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Papas>()
                .Property(p => p.PrecioMediano)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Papas>()
                .Property(p => p.PrecioGrande)
                .HasPrecision(18, 2);

            // ✅ NUEVO: Configuración para ComboBebida
            modelBuilder.Entity<ComboBebida>()
                .Property(cb => cb.RecargoPequeno)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ComboBebida>()
                .Property(cb => cb.RecargoMediano)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ComboBebida>()
                .Property(cb => cb.RecargoGrande)
                .HasPrecision(18, 2);

            // Configuración de herencia TPH
            modelBuilder.Entity<Producto>()
                .HasDiscriminator<string>("TipoProducto")
                .HasValue<Hamburguesa>("Hamburguesa")
                .HasValue<Papas>("Papas")
                .HasValue<Bebida>("Bebida");

            // Configuración de relaciones
            modelBuilder.Entity<ItemCombo>()
                .HasOne(ic => ic.Combo)
                .WithMany(c => c.Items)
                .HasForeignKey(ic => ic.ComboId);

            modelBuilder.Entity<ItemCombo>()
                .HasOne(ic => ic.Producto)
                .WithMany()
                .HasForeignKey(ic => ic.ProductoId);

            // ✅ NUEVO: Configuración de relaciones para ComboBebida
            modelBuilder.Entity<ComboBebida>()
                .HasKey(cb => cb.Id);

            modelBuilder.Entity<ComboBebida>()
                .HasOne(cb => cb.Combo)
                .WithMany(c => c.BebidasDisponibles)
                .HasForeignKey(cb => cb.ComboId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComboBebida>()
                .HasOne(cb => cb.Bebida)
                .WithMany()
                .HasForeignKey(cb => cb.BebidaId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ NUEVO: Índice único para evitar duplicados en ComboBebida
            modelBuilder.Entity<ComboBebida>()
                .HasIndex(cb => new { cb.ComboId, cb.BebidaId })
                .IsUnique();


            // Relación muchos a muchos para ingredientes
            modelBuilder.Entity<IngredienteHamburguesa>()
                .HasKey(ih => ih.Id);

            modelBuilder.Entity<IngredienteHamburguesa>()
                .HasOne(ih => ih.Hamburguesa)
                .WithMany(h => h.Ingredientes)
                .HasForeignKey(ih => ih.HamburguesaId);

            modelBuilder.Entity<IngredienteHamburguesa>()
                .HasOne(ih => ih.Ingrediente)
                .WithMany(i => i.Hamburguesas)
                .HasForeignKey(ih => ih.IngredienteId);

            // Seed data (mantener la llamada si tienes seed data separado)
            modelBuilder.Seed();
        }
    }
}