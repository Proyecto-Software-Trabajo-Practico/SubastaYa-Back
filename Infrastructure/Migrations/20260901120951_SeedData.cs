using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Nombre", "UrlIcono" },
                values: new object[,]
                {
                    { 1, "Tecnología", null },
                    { 2, "Coleccionables", null },
                    { 3, "Indumentaria", null },
                    { 4, "Vehículos", null }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "FechaRegistro", "Nombre", "PasswordHash" },
                values: new object[,]
                {
                    { 1, "vendedor@test.com", new DateTime(2026, 9, 1, 12, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), "Vendedor Test", "hashed_pw" },
                    { 2, "comprador1@test.com", new DateTime(2026, 9, 1, 12, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), "Comprador Líder", "hashed_pw" },
                    { 3, "comprador2@test.com", new DateTime(2026, 9, 1, 12, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), "Comprador Habilitado", "hashed_pw" },
                    { 4, "sinfondos@test.com", new DateTime(2026, 9, 1, 12, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), "Comprador Sin Fondos", "hashed_pw" }
                });

            migrationBuilder.InsertData(
                table: "Billeteras",
                columns: new[] { "Id", "SaldoDisponible", "SaldoRetenido", "SaldoTotal", "UsuarioId", "Version" },
                values: new object[,]
                {
                    { 1, 0m, 0m, 0m, 1, 1 },
                    { 2, 105000m, 45000m, 150000m, 2, 1 },
                    { 3, 200000m, 0m, 200000m, 3, 1 },
                    { 4, 500m, 0m, 500m, 4, 1 }
                });

            migrationBuilder.InsertData(
                table: "Subastas",
                columns: new[] { "Id", "CategoriaId", "Descripcion", "Estado", "FechaFin", "FechaInicio", "IncrementoMinimo", "PrecioBase", "Titulo", "UrlImagen", "VendedorId", "Version" },
                values: new object[,]
                {
                    { 1, 1, "Subasta con 2 pujas.", "ACTIVA", new DateTime(2026, 9, 1, 12, 34, 51, 84, DateTimeKind.Utc).AddTicks(7368), new DateTime(2026, 9, 1, 11, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), 5000m, 10000m, "Activa Estándar", null, 1, 1 },
                    { 2, 1, "Alerta visual anti-sniping.", "ACTIVA", new DateTime(2026, 9, 1, 12, 10, 51, 84, DateTimeKind.Utc).AddTicks(7368), new DateTime(2026, 9, 1, 11, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), 1000m, 5000m, "Activa Crítica", null, 1, 1 },
                    { 3, 2, "Inicia mañana.", "PROGRAMADA", new DateTime(2026, 9, 3, 12, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), new DateTime(2026, 9, 2, 12, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), 2000m, 20000m, "Próxima", null, 1, 1 },
                    { 4, 3, "Esperando liquidación.", "ACTIVA", new DateTime(2026, 8, 31, 12, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), new DateTime(2026, 8, 30, 12, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), 500m, 5000m, "Vencida con Ganador", null, 1, 1 },
                    { 5, 4, "Nadie pujó.", "ACTIVA", new DateTime(2026, 8, 31, 12, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), new DateTime(2026, 8, 30, 12, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), 5000m, 50000m, "Vencida Desierta", null, 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "Pujas",
                columns: new[] { "Id", "CompradorId", "FechaPuja", "Monto", "SubastaId" },
                values: new object[,]
                {
                    { 1, 3, new DateTime(2026, 9, 1, 11, 49, 51, 84, DateTimeKind.Utc).AddTicks(7368), 20000m, 1 },
                    { 2, 2, new DateTime(2026, 9, 1, 12, 4, 51, 84, DateTimeKind.Utc).AddTicks(7368), 45000m, 1 },
                    { 3, 3, new DateTime(2026, 8, 31, 11, 59, 51, 84, DateTimeKind.Utc).AddTicks(7368), 6000m, 4 }
                });

            migrationBuilder.InsertData(
                table: "TransaccionesLedger",
                columns: new[] { "Id", "BilleteraId", "Fecha", "Monto", "SubastaId", "Tipo" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2026, 8, 31, 12, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), 150000m, null, "DEPOSITO" },
                    { 2, 2, new DateTime(2026, 9, 1, 12, 4, 51, 84, DateTimeKind.Utc).AddTicks(7368), 45000m, 1, "RETENCION" },
                    { 3, 3, new DateTime(2026, 8, 31, 12, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), 200000m, null, "DEPOSITO" },
                    { 4, 4, new DateTime(2026, 8, 31, 12, 9, 51, 84, DateTimeKind.Utc).AddTicks(7368), 500m, null, "DEPOSITO" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
