using System.ComponentModel.DataAnnotations;

namespace IntercoopAPI.Models.DTOs
{
    public class SolicitudCreateDto
    {
        [Required]
        public int IdUsuarioSolicitante { get; set; }
        [Required]
        public int IdCategoria { get; set; }
        [Required]
        public string Titulo { get; set; } = string.Empty;
        [Required]
        public string Descripcion { get; set; } = string.Empty;
    }

    public class SolicitudUpdateDto
    {
        [Required]
        public string Estado { get; set; } = string.Empty;
    }
}