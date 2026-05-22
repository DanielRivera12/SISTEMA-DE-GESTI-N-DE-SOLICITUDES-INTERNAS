using Microsoft.EntityFrameworkCore;
using IntercoopAPI.Models.Domain;

namespace IntercoopAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor que recibe las opciones de conexión desde Program.cs
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Mapeo de las entidades a las tablas de la base de datos
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuramos las llaves primarias explícitamente por buena práctica
            modelBuilder.Entity<Rol>().HasKey(r => r.IdRol);
            modelBuilder.Entity<Categoria>().HasKey(c => c.IdCategoria);
            modelBuilder.Entity<Usuario>().HasKey(u => u.IdUsuario);
            modelBuilder.Entity<Solicitud>().HasKey(s => s.IdSolicitud);

            // FILTROS GLOBALES PARA ELIMINACIÓN LÓGICA
            // Esto asegura que cualquier consulta (SELECT) ignore automáticamente los registros inactivos
            modelBuilder.Entity<Rol>().HasQueryFilter(r => r.Activo);
            modelBuilder.Entity<Categoria>().HasQueryFilter(c => c.Activo);
            modelBuilder.Entity<Usuario>().HasQueryFilter(u => u.Activo);
            modelBuilder.Entity<Solicitud>().HasQueryFilter(s => s.Activo);
        }
    }
}