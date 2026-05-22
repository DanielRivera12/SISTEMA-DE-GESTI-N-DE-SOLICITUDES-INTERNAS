using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntercoopAPI.Data;
using IntercoopAPI.Models.Domain;
using IntercoopAPI.Models.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IntercoopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public UsuariosController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // 1. OBTENER USUARIOS (Para el ABC)
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol) // Traemos los datos del rol
                .Select(u => new { u.IdUsuario, u.Nombre, u.Correo, Rol = u.Rol.Nombre })
                .ToListAsync();
            
            return Ok(usuarios);
        }

        // 2. CREAR USUARIO (ABC)
        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] UsuarioCreateDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
                return BadRequest("El correo ya está registrado.");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                IdRol = dto.IdRol,
                // Encriptamos la contraseña con BCrypt
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado con éxito." });
        }

        // 3. LOGIN (Generación de JWT)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == dto.Correo);

            // Verificamos usuario y contraseña
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
                return Unauthorized("Credenciales inválidas.");

            // Generamos el Token JWT
            var token = GenerarJwtToken(usuario);

            return Ok(new { token, rol = usuario.Rol.Nombre });
        }

        // 4. ELIMINACIÓN LÓGICA DE USUARIO
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound("Usuario no encontrado.");

            usuario.Activo = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario eliminado lógicamente." });
        }

        // MÉTODO PRIVADO PARA GENERAR EL TOKEN
        private string GenerarJwtToken(Usuario usuario)
        {
            // Usamos una clave secreta por defecto si no está en el appsettings
            var jwtKey = _config["Jwt:Key"] ?? "ClaveSuperSecretaParaIntercoop2026ExamenPractico!";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre) // Añadimos el rol al token
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"] ?? "IntercoopAPI",
                audience: _config["Jwt:Audience"] ?? "IntercoopUsers",
                claims: claims,
                expires: DateTime.Now.AddHours(5), // El token expira en 5 horas
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}