using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntercoopAPI.Data;
using IntercoopAPI.Models.Domain;
using IntercoopAPI.Models.DTOs;
using IntercoopAPI.Services;

namespace IntercoopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Exige que el usuario envíe un Token JWT válido
    public class SolicitudesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public SolicitudesController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // 1. OBTENER SOLICITUDES CON FILTROS AVANZADOS
        [HttpGet]
        public async Task<IActionResult> GetSolicitudes(
            [FromQuery] string? estado, 
            [FromQuery] int? idCategoria, 
            [FromQuery] DateTime? fechaInicio, 
            [FromQuery] DateTime? fechaFin)
        {
            // Usamos IQueryable para construir la consulta dinámicamente sin traer todos los datos a memoria aún
            var query = _context.Solicitudes
                .Include(s => s.UsuarioSolicitante)
                .Include(s => s.Categoria)
                .AsQueryable();

            // Aplicar filtros dinámicos si el usuario los envía
            if (!string.IsNullOrEmpty(estado))
                query = query.Where(s => s.Estado.ToLower() == estado.ToLower());

            if (idCategoria.HasValue)
                query = query.Where(s => s.IdCategoria == idCategoria.Value);

            if (fechaInicio.HasValue)
                query = query.Where(s => s.FechaCreacion >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(s => s.FechaCreacion <= fechaFin.Value);

            // Proyectamos el resultado para no enviar la contraseña del usuario ni datos innecesarios
            var solicitudes = await query.Select(s => new {
                s.IdSolicitud,
                s.Titulo,
                s.Descripcion,
                s.Estado,
                s.FechaCreacion,
                Categoria = s.Categoria.Nombre,
                Solicitante = s.UsuarioSolicitante.Nombre,
                CorreoSolicitante = s.UsuarioSolicitante.Correo
            }).ToListAsync();

            return Ok(solicitudes);
        }

        // 2. CREAR SOLICITUD (Automáticamente dispara correo)
        [HttpPost]
        public async Task<IActionResult> CreateSolicitud([FromBody] SolicitudCreateDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(dto.IdUsuarioSolicitante);
            if (usuario == null) return NotFound("Usuario solicitante no existe.");

            var solicitud = new Solicitud
            {
                IdUsuarioSolicitante = dto.IdUsuarioSolicitante,
                IdCategoria = dto.IdCategoria,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Estado = "Pendiente" // Estado inicial por defecto
            };

            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();

            // Enviar notificación por correo
            string mensaje = $"Hola {usuario.Nombre}, tu solicitud '{solicitud.Titulo}' ha sido registrada con el estado: Pendiente.";
            await _emailService.EnviarCorreoAsync(usuario.Correo, "Nueva Solicitud Registrada", mensaje);

            return Ok(new { message = "Solicitud creada exitosamente.", id = solicitud.IdSolicitud });
        }

        // 3. ACTUALIZAR ESTADO (Solo Operadores o Administradores)
        [HttpPut("{id}/estado")]
        [Authorize(Roles = "Administrador,Operador")] // Restricción por roles
        public async Task<IActionResult> UpdateEstado(int id, [FromBody] SolicitudUpdateDto dto)
        {
            var solicitud = await _context.Solicitudes
                .Include(s => s.UsuarioSolicitante)
                .FirstOrDefaultAsync(s => s.IdSolicitud == id);

            if (solicitud == null) return NotFound("Solicitud no encontrada.");

            solicitud.Estado = dto.Estado;
            solicitud.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Enviar notificación de actualización
            string mensaje = $"Hola {solicitud.UsuarioSolicitante.Nombre}, el estado de tu solicitud '{solicitud.Titulo}' ha cambiado a: {dto.Estado}.";
            await _emailService.EnviarCorreoAsync(solicitud.UsuarioSolicitante.Correo, "Actualización de Solicitud", mensaje);

            return Ok(new { message = $"Estado actualizado a {dto.Estado}" });
        }

        // 4. ELIMINACIÓN LÓGICA
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")] // Solo admin puede eliminar
        public async Task<IActionResult> DeleteSolicitud(int id)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null) return NotFound("Solicitud no encontrada.");

            solicitud.Activo = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Solicitud eliminada lógicamente." });
        }
    }
}