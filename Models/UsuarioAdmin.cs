// Models/UsuarioAdmin.cs
namespace Restaurante.Models
{
    public class UsuarioAdmin
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Nombre { get; set; }
        public RolUsuario Rol { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? UltimoAcceso { get; set; }
    }

    public enum RolUsuario
    {
        SuperAdmin,
        Cocina
    }
}