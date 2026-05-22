using System.ComponentModel.DataAnnotations;

namespace IntercoopAPI.Models.DTOs
{
    public class CategoriaCreateDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
        
        public string? Descripcion { get; set; }
    }

    public class CategoriaUpdateDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
        
        public string? Descripcion { get; set; }
    }
}
