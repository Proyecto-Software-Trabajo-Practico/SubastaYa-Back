using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Persistence
{
    public static class DataSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var now = DateTime.UtcNow;

            // 1. USUARIOS (Adaptados a ASP.NET Core Identity)
            modelBuilder.Entity<Usuario>().HasData(
                new
                {
                    Id = 1,
                    Email = "vendedor@test.com",
                    NormalizedEmail = "VENDEDOR@TEST.COM",
                    UserName = "vendedor@test.com",
                    NormalizedUserName = "VENDEDOR@TEST.COM",
                    EmailConfirmed = true,
                    Nombre = "Vendedor Test",
                    PasswordHash = "AQAAAAIAAYagAAAAE...", // Dummy hash
                    SecurityStamp = "d870c9bc-6b08-4171-bc01-1b07221f42d2",
                    ConcurrencyStamp = "f16599b5-7798-4b71-9b7e-90f7d542138b",
                    AccessFailedCount = 0,
                    LockoutEnabled = false,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    FechaRegistro = now
                },
                new
                {
                    Id = 2,
                    Email = "comprador1@test.com",
                    NormalizedEmail = "COMPRADOR1@TEST.COM",
                    UserName = "comprador1@test.com",
                    NormalizedUserName = "COMPRADOR1@TEST.COM",
                    EmailConfirmed = true,
                    Nombre = "Comprador Líder",
                    PasswordHash = "AQAAAAIAAYagAAAAE...",
                    SecurityStamp = "c93a0d58-9588-4660-843e-f8316dfa99aa",
                    ConcurrencyStamp = "a72d3f9b-648b-4b2b-a01f-0e42f9d8a11b",
                    AccessFailedCount = 0,
                    LockoutEnabled = false,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    FechaRegistro = now
                },
                new
                {
                    Id = 3,
                    Email = "comprador2@test.com",
                    NormalizedEmail = "COMPRADOR2@TEST.COM",
                    UserName = "comprador2@test.com",
                    NormalizedUserName = "COMPRADOR2@TEST.COM",
                    EmailConfirmed = true,
                    Nombre = "Comprador Habilitado",
                    PasswordHash = "AQAAAAIAAYagAAAAE...",
                    SecurityStamp = "b82a1e7d-1234-4567-890a-bcdef1234567",
                    ConcurrencyStamp = "e9123456-789a-bcde-f012-3456789abcde",
                    AccessFailedCount = 0,
                    LockoutEnabled = false,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    FechaRegistro = now
                },
                new
                {
                    Id = 4,
                    Email = "sinfondos@test.com",
                    NormalizedEmail = "SINFONDOS@TEST.COM",
                    UserName = "sinfondos@test.com",
                    NormalizedUserName = "SINFONDOS@TEST.COM",
                    EmailConfirmed = true,
                    Nombre = "Comprador Sin Fondos",
                    PasswordHash = "AQAAAAIAAYagAAAAE...",
                    SecurityStamp = "a11b22c3-33d4-44e5-55f6-66a77b88c99d",
                    ConcurrencyStamp = "f00e99d8-88c7-77b6-66a5-55d44c33b22a",
                    AccessFailedCount = 0,
                    LockoutEnabled = false,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    FechaRegistro = now
                }
            );

            // 2. BILLETERAS
            modelBuilder.Entity<Billetera>().HasData(
                new { Id = 1, UsuarioId = 1, SaldoTotal = 0m, SaldoRetenido = 0m, SaldoDisponible = 0m, Version = 1 },
                new { Id = 2, UsuarioId = 2, SaldoTotal = 150000m, SaldoRetenido = 45000m, SaldoDisponible = 105000m, Version = 1 },
                new { Id = 3, UsuarioId = 3, SaldoTotal = 200000m, SaldoRetenido = 0m, SaldoDisponible = 200000m, Version = 1 },
                new { Id = 4, UsuarioId = 4, SaldoTotal = 500m, SaldoRetenido = 0m, SaldoDisponible = 500m, Version = 1 }
            );

            // 3. CATEGORÍAS
            modelBuilder.Entity<Categoria>().HasData(
                new { Id = 1, Nombre = "Tecnología" },
                new { Id = 2, Nombre = "Coleccionables" },
                new { Id = 3, Nombre = "Indumentaria" },
                new { Id = 4, Nombre = "Vehículos" }
            );

            // 4. SUBASTAS
            modelBuilder.Entity<Subasta>().HasData(
                new { Id = 1, VendedorId = 1, CategoriaId = 1, Titulo = "Activa Estándar", Descripcion = "Subasta con 2 pujas.", PrecioBase = 10000m, IncrementoMinimo = 5000m, FechaInicio = now.AddHours(-1), FechaFin = now.AddMinutes(25), Estado = "ACTIVA", Version = 1 },
                new { Id = 2, VendedorId = 1, CategoriaId = 1, Titulo = "Activa Crítica", Descripcion = "Alerta visual anti-sniping.", PrecioBase = 5000m, IncrementoMinimo = 1000m, FechaInicio = now.AddHours(-1), FechaFin = now.AddMinutes(1), Estado = "ACTIVA", Version = 1 },
                new { Id = 3, VendedorId = 1, CategoriaId = 2, Titulo = "Próxima", Descripcion = "Inicia mañana.", PrecioBase = 20000m, IncrementoMinimo = 2000m, FechaInicio = now.AddHours(24), FechaFin = now.AddHours(48), Estado = "PROGRAMADA", Version = 1 },
                new { Id = 4, VendedorId = 1, CategoriaId = 3, Titulo = "Vencida con Ganador", Descripcion = "Esperando liquidación.", PrecioBase = 5000m, IncrementoMinimo = 500m, FechaInicio = now.AddDays(-2), FechaFin = now.AddDays(-1), Estado = "ACTIVA", Version = 1 },
                new { Id = 5, VendedorId = 1, CategoriaId = 4, Titulo = "Vencida Desierta", Descripcion = "Nadie pujó.", PrecioBase = 50000m, IncrementoMinimo = 5000m, FechaInicio = now.AddDays(-2), FechaFin = now.AddDays(-1), Estado = "ACTIVA", Version = 1 }
            );

            // 5. PUJAS
            modelBuilder.Entity<Puja>().HasData(
                new { Id = 1, SubastaId = 1, CompradorId = 3, Monto = 20000m, FechaPuja = now.AddMinutes(-20) },
                new { Id = 2, SubastaId = 1, CompradorId = 2, Monto = 45000m, FechaPuja = now.AddMinutes(-5) },
                new { Id = 3, SubastaId = 4, CompradorId = 3, Monto = 6000m, FechaPuja = now.AddDays(-1).AddMinutes(-10) }
            );

            // 6. REGISTROS CONTABLES (Ledger)
            modelBuilder.Entity<TransaccionLedger>().HasData(
                new { Id = 1, BilleteraId = 2, Tipo = "DEPOSITO", Monto = 150000m, Fecha = now.AddDays(-1) },
                new { Id = 2, BilleteraId = 2, Tipo = "RETENCION", Monto = 45000m, Fecha = now.AddMinutes(-5), SubastaId = 1 },
                new { Id = 3, BilleteraId = 3, Tipo = "DEPOSITO", Monto = 200000m, Fecha = now.AddDays(-1) },
                new { Id = 4, BilleteraId = 4, Tipo = "DEPOSITO", Monto = 500m, Fecha = now.AddDays(-1) }
            );
        }
    }
}