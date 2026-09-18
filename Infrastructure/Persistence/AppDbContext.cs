using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Billetera> Billeteras { get; set; } = null!;
    public DbSet<Categoria> Categorias { get; set; } = null!;
    public DbSet<Subasta> Subastas { get; set; } = null!;
    public DbSet<Puja> Pujas { get; set; } = null!;
    public DbSet<TransaccionLedger> TransaccionesLedger { get; set; } = null!;
    public DbSet<Auditoria> Auditorias { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios"); 

            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categorias");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Billetera>(entity =>
        {
            entity.ToTable("Billeteras");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SaldoTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SaldoRetenido).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SaldoDisponible).HasColumnType("decimal(18,2)");

            // Mapeo a timestamp/rowversion nativo de SQL Server
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.Usuario)
                .WithOne()
                .HasForeignKey<Billetera>(b => b.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Subasta>(entity =>
        {
            entity.ToTable("Subastas");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PrecioBase).HasColumnType("decimal(18,2)");
            entity.Property(e => e.IncrementoMinimo).HasColumnType("decimal(18,2)");

            // Índices para optimizar consultas de catálogo y barrido del Background Worker
            entity.HasIndex(e => e.Estado);
            entity.HasIndex(e => e.FechaFin);

            // Mapeo a timestamp/rowversion nativo de SQL Server
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasOne(e => e.Vendedor)
                  .WithMany()
                  .HasForeignKey(e => e.VendedorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Categoria)
                  .WithMany()
                  .HasForeignKey(e => e.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Puja>(entity =>
        {
            entity.ToTable("Pujas");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Monto).HasColumnType("decimal(18,2)");
            entity.Property(e => e.FechaPuja).HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.RowVersion)
                  .IsRowVersion();

            entity.HasOne(e => e.Subasta)
                  .WithMany(s => s.Pujas)
                  .HasForeignKey(e => e.SubastaId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Comprador)
                  .WithMany()
                  .HasForeignKey(e => e.CompradorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

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

        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.ToTable("Auditorias");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Fecha).HasDefaultValueSql("GETUTCDATE()");

            // Índice compuesto para agilizar búsquedas de auditoría por entidad
            entity.HasIndex(e => new { e.Entidad, e.EntidadId });

            entity.HasOne(e => e.Usuario)
                  .WithMany()
                  .HasForeignKey(e => e.UsuarioId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        DataSeeder.Seed(modelBuilder);
    }

    // Sobrescribimos SaveChangesAsync para evitar modificaciones o eliminaciones de registros de auditoria
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var auditoriasModificadas = ChangeTracker.Entries<Auditoria>()
            .Any(e => e.State == EntityState.Modified || e.State == EntityState.Deleted);

        if (auditoriasModificadas)
        {
            throw new InvalidOperationException("Los registros de auditor�a son inmutables y no pueden ser modificados ni eliminados.");
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
