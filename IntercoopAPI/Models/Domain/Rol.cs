using System.ComponentModel.DataAnnotations;

namespace IntercoopAPI.Models.Domain
{
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
}