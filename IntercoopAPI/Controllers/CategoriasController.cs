using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntercoopAPI.Data;
using IntercoopAPI.Models.Domain;
using IntercoopAPI.Models.DTOs;

namespace IntercoopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. OBTENER TODAS LAS CATEGORÍAS ACTIVAS (El filtro global hace el trabajo)
        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            var categorias = await _context.Categorias.ToListAsync();
            return Ok(categorias);
        }

        // 2. CREAR UNA CATEGORÍA
        [HttpPost]
        public async Task<IActionResult> CreateCategoria([FromBody] CategoriaCreateDto dto)
        {
            // Validación de unicidad
            if (await _context.Categorias.AnyAsync(c => c.Nombre == dto.Nombre))
                return BadRequest("Ya existe una categoría con este nombre.");

            var categoria = new Categoria
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Categoría creada con éxito", data = categoria });
        }

        // 3. ACTUALIZAR UNA CATEGORÍA
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoria(int id, [FromBody] CategoriaUpdateDto dto)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound("Categoría no encontrada.");

            categoria.Nombre = dto.Nombre;
            categoria.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Categoría actualizada", data = categoria });
        }

        // 4. ELIMINACIÓN LÓGICA DE UNA CATEGORÍA
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound("Categoría no encontrada.");

            // Cumpliendo el requerimiento de eliminación lógica
            categoria.Activo = false; 
            await _context.SaveChangesAsync();

            return Ok(new { message = "Categoría eliminada lógicamente." });
        }
    }
}