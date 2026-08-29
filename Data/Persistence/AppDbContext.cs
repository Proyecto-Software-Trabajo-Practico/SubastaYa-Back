using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Mapea aquí tus entidades de la capa Domain
        // public DbSet<Subasta> Subastas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuraciones de entidades si es necesario
        }
    }
}
