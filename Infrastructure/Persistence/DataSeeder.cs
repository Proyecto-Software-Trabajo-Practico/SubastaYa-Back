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

            // 1. USUARIOS
            modelBuilder.Entity<Usuario>().HasData(
                new { Id = 1, Email = "vendedor@test.com", Nombre = "Vendedor Test", PasswordHash = "hashed_pw", FechaRegistro = now },
                new { Id = 2, Email = "comprador1@test.com", Nombre = "Comprador Líder", PasswordHash = "hashed_pw", FechaRegistro = now },
                new { Id = 3, Email = "comprador2@test.com", Nombre = "Comprador Habilitado", PasswordHash = "hashed_pw", FechaRegistro = now },
                new { Id = 4, Email = "sinfondos@test.com", Nombre = "Comprador Sin Fondos", PasswordHash = "hashed_pw", FechaRegistro = now }
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
                // 1. Activa estándar: Cierra en 20-30 min
                new { Id = 1, VendedorId = 1, CategoriaId = 1, Titulo = "Activa Estándar", Descripcion = "Subasta con 2 pujas.", PrecioBase = 10000m, IncrementoMinimo = 5000m, FechaInicio = now.AddHours(-1), FechaFin = now.AddMinutes(25), Estado = "ACTIVA", Version = 1 },
                
                // 2. Activa crítica: Cierra en menos de 2 min
                new { Id = 2, VendedorId = 1, CategoriaId = 1, Titulo = "Activa Crítica", Descripcion = "Alerta visual anti-sniping.", PrecioBase = 5000m, IncrementoMinimo = 1000m, FechaInicio = now.AddHours(-1), FechaFin = now.AddMinutes(1), Estado = "ACTIVA", Version = 1 },
                
                // 3. Próxima: Inicio programado a +24 hs
                new { Id = 3, VendedorId = 1, CategoriaId = 2, Titulo = "Próxima", Descripcion = "Inicia mañana.", PrecioBase = 20000m, IncrementoMinimo = 2000m, FechaInicio = now.AddHours(24), FechaFin = now.AddHours(48), Estado = "PROGRAMADA", Version = 1 },
                
                // 4. Vencida con ganador: Fecha fin pasada (Para el worker)
                new { Id = 4, VendedorId = 1, CategoriaId = 3, Titulo = "Vencida con Ganador", Descripcion = "Esperando liquidación.", PrecioBase = 5000m, IncrementoMinimo = 500m, FechaInicio = now.AddDays(-2), FechaFin = now.AddDays(-1), Estado = "ACTIVA", Version = 1 },
                
                // 5. Vencida desierta: Fecha fin pasada sin pujas
                new { Id = 5, VendedorId = 1, CategoriaId = 4, Titulo = "Vencida Desierta", Descripcion = "Nadie pujó.", PrecioBase = 50000m, IncrementoMinimo = 5000m, FechaInicio = now.AddDays(-2), FechaFin = now.AddDays(-1), Estado = "ACTIVA", Version = 1 }
            );

            // 5. PUJAS (Para Subasta 1)
            modelBuilder.Entity<Puja>().HasData(
                new { Id = 1, SubastaId = 1, CompradorId = 3, Monto = 20000m, FechaPuja = now.AddMinutes(-20) }, // Oferta previa
                new { Id = 2, SubastaId = 1, CompradorId = 2, Monto = 45000m, FechaPuja = now.AddMinutes(-5) },  // Postor Líder
                new { Id = 3, SubastaId = 4, CompradorId = 3, Monto = 6000m, FechaPuja = now.AddDays(-1).AddMinutes(-10) } // Ganador de la vencida
            );

            // 6. REGISTROS CONTABLES (Ledger)
            modelBuilder.Entity<TransaccionLedger>().HasData(
                // Depósito inicial de Comprador 1 y su retención de 45k para la Subasta 1
                new { Id = 1, BilleteraId = 2, Tipo = "DEPOSITO", Monto = 150000m, Fecha = now.AddDays(-1) },
                new { Id = 2, BilleteraId = 2, Tipo = "RETENCION", Monto = 45000m, Fecha = now.AddMinutes(-5), SubastaId = 1 },
                
                // Depósito inicial de Comprador 2
                new { Id = 3, BilleteraId = 3, Tipo = "DEPOSITO", Monto = 200000m, Fecha = now.AddDays(-1) },
                
                // Depósito de Sin Fondos
                new { Id = 4, BilleteraId = 4, Tipo = "DEPOSITO", Monto = 500m, Fecha = now.AddDays(-1) }
            );
        }
    }
}
