using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Billetera> Billeteras { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Subasta> Subastas { get; set; } = null!;
        public DbSet<Puja> Pujas { get; set; } = null!;
        public DbSet<TransaccionLedger> TransaccionesLedger { get; set; } = null!;
        public DbSet<Auditoria> Auditorias { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USUARIO
            modelBuilder.Entity<Usuario>(entity => 
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired();
                
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETUTCDATE()");
            });

            // BILLETERA
            modelBuilder.Entity<Billetera>(entity =>
            {
                entity.ToTable("Billeteras");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.SaldoTotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SaldoRetenido).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SaldoDisponible).HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.Version).IsConcurrencyToken();

                entity.HasOne(e => e.Usuario)
                    .WithOne()
                    .HasForeignKey<Billetera>(b => b.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CATEGORIA
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("Categorias");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            });

            // SUBASTA
            modelBuilder.Entity<Subasta>(entity =>
            {
                entity.ToTable("Subastas");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PrecioBase).HasColumnType("decimal(18,2)");
                entity.Property(e => e.IncrementoMinimo).HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.Version).IsConcurrencyToken();

                entity.HasOne(e => e.Vendedor)
                      .WithMany()
                      .HasForeignKey(e => e.VendedorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Categoria)
                      .WithMany()
                      .HasForeignKey(e => e.CategoriaId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // PUJA
            modelBuilder.Entity<Puja>(entity =>
            {
                entity.ToTable("Pujas");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Monto).HasColumnType("decimal(18,2)");
                entity.Property(e => e.FechaPuja).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasOne(e => e.Subasta)
                      .WithMany(s => s.Pujas)
                      .HasForeignKey(e => e.SubastaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Comprador)
                      .WithMany()
                      .HasForeignKey(e => e.CompradorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // TRANSACCION_LEDGER
            modelBuilder.Entity<TransaccionLedger>(entity =>
            {
                entity.ToTable("TransaccionesLedger");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Tipo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Monto).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Fecha).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Billetera)
                      .WithMany(b => b.Transacciones)
                      .HasForeignKey(e => e.BilleteraId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Subasta>()
                      .WithMany()
                      .HasForeignKey(e => e.SubastaId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // AUDITORIA
            modelBuilder.Entity<Auditoria>(entity =>
            {
                entity.ToTable("Auditorias");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Fecha).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
