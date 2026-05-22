using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntercoopAPI.Data;

namespace IntercoopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador,Operador")] // Restringimos el acceso al dashboard
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen()
        {
            // 1. Conteo general y por estados
            var totalSolicitudes = await _context.Solicitudes.CountAsync();
            
            var pendientes = await _context.Solicitudes.CountAsync(s => s.Estado == "Pendiente");
            var enProceso = await _context.Solicitudes.CountAsync(s => s.Estado == "En Proceso");
            var completadas = await _context.Solicitudes.CountAsync(s => s.Estado == "Completado");
            var rechazadas = await _context.Solicitudes.CountAsync(s => s.Estado == "Rechazado");

            // 2. Agrupación por Categorías (Muy útil para gráficas de pastel o barras)
            var solicitudesPorCategoria = await _context.Solicitudes
                .GroupBy(s => s.Categoria!.Nombre)
                .Select(g => new 
                { 
                    Categoria = g.Key, 
                    Cantidad = g.Count() 
                })
                .ToListAsync();

            // 3. Últimas 5 solicitudes recientes (Para mostrar una tabla rápida en el inicio)
            var solicitudesRecientes = await _context.Solicitudes
                .Include(s => s.UsuarioSolicitante)
                .OrderByDescending(s => s.FechaCreacion)
                .Take(5)
                .Select(s => new {
                    s.IdSolicitud,
                    s.Titulo,
                    Solicitante = s.UsuarioSolicitante!.Nombre,
                    s.Estado,
                    s.FechaCreacion
                })
                .ToListAsync();

            // Construimos el objeto final (JSON) que consumirá el Frontend
            var dashboardData = new
            {
                EstadisticasGenerales = new
                {
                    Total = totalSolicitudes,
                    Pendientes = pendientes,
                    EnProceso = enProceso,
                    Completadas = completadas,
                    Rechazadas = rechazadas
                },
                DistribucionPorCategoria = solicitudesPorCategoria,
                Recientes = solicitudesRecientes
            };

            return Ok(dashboardData);
        }
    }
}