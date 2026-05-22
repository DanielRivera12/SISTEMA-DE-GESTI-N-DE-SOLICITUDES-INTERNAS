using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntercoopAPI.Models.Domain
{
    public class Solicitud
    {
        [Key]
        public int IdSolicitud { get; set; }
        
        public int IdUsuarioSolicitante { get; set; }
        public int IdCategoria { get; set; }
        
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = "Pendiente"; 
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaModificacion { get; set; }
        public bool Activo { get; set; } = true;

        // Enlaces explícitos para que no invente columnas
        [ForeignKey("IdUsuarioSolicitante")]
        public Usuario? UsuarioSolicitante { get; set; }

        [ForeignKey("IdCategoria")]
        public Categoria? Categoria { get; set; }
    }
}