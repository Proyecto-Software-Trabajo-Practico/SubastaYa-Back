using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CambiarRowVersionBytes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Subastas");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Billeteras");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Subastas",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Billeteras",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaPuja",
                value: new DateTime(2026, 9, 14, 2, 44, 9, 580, DateTimeKind.Utc).AddTicks(9714));

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaPuja",
                value: new DateTime(2026, 9, 14, 2, 59, 9, 580, DateTimeKind.Utc).AddTicks(9714));

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaPuja",
                value: new DateTime(2026, 9, 13, 2, 54, 9, 580, DateTimeKind.Utc).AddTicks(9714));

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 14, 3, 29, 9, 580, DateTimeKind.Utc).AddTicks(9714), new DateTime(2026, 9, 14, 2, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 14, 3, 5, 9, 580, DateTimeKind.Utc).AddTicks(9714), new DateTime(2026, 9, 14, 2, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 16, 3, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714), new DateTime(2026, 9, 15, 3, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 13, 3, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714), new DateTime(2026, 9, 12, 3, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 13, 3, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714), new DateTime(2026, 9, 12, 3, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714) });

            migrationBuilder.UpdateData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 1,
                column: "Fecha",
                value: new DateTime(2026, 9, 13, 3, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714));

            migrationBuilder.UpdateData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 2,
                column: "Fecha",
                value: new DateTime(2026, 9, 14, 2, 59, 9, 580, DateTimeKind.Utc).AddTicks(9714));

            migrationBuilder.UpdateData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 3,
                column: "Fecha",
                value: new DateTime(2026, 9, 13, 3, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714));

            migrationBuilder.UpdateData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 4,
                column: "Fecha",
                value: new DateTime(2026, 9, 13, 3, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 14, 3, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 14, 3, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 14, 3, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 14, 3, 4, 9, 580, DateTimeKind.Utc).AddTicks(9714));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Subastas");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Billeteras");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Subastas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Billeteras",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 3,
                column: "Version",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 4,
                column: "Version",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaPuja",
                value: new DateTime(2026, 9, 12, 22, 8, 12, 785, DateTimeKind.Utc).AddTicks(797));

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaPuja",
                value: new DateTime(2026, 9, 12, 22, 23, 12, 785, DateTimeKind.Utc).AddTicks(797));

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaPuja",
                value: new DateTime(2026, 9, 11, 22, 18, 12, 785, DateTimeKind.Utc).AddTicks(797));

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaFin", "FechaInicio", "Version" },
                values: new object[] { new DateTime(2026, 9, 12, 22, 53, 12, 785, DateTimeKind.Utc).AddTicks(797), new DateTime(2026, 9, 12, 21, 28, 12, 785, DateTimeKind.Utc).AddTicks(797), 1 });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaFin", "FechaInicio", "Version" },
                values: new object[] { new DateTime(2026, 9, 12, 22, 29, 12, 785, DateTimeKind.Utc).AddTicks(797), new DateTime(2026, 9, 12, 21, 28, 12, 785, DateTimeKind.Utc).AddTicks(797), 1 });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaFin", "FechaInicio", "Version" },
                values: new object[] { new DateTime(2026, 9, 14, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797), new DateTime(2026, 9, 13, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797), 1 });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaFin", "FechaInicio", "Version" },
                values: new object[] { new DateTime(2026, 9, 11, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797), new DateTime(2026, 9, 10, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797), 1 });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaFin", "FechaInicio", "Version" },
                values: new object[] { new DateTime(2026, 9, 11, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797), new DateTime(2026, 9, 10, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797), 1 });

            migrationBuilder.UpdateData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 1,
                column: "Fecha",
                value: new DateTime(2026, 9, 11, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797));

            migrationBuilder.UpdateData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 2,
                column: "Fecha",
                value: new DateTime(2026, 9, 12, 22, 23, 12, 785, DateTimeKind.Utc).AddTicks(797));

            migrationBuilder.UpdateData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 3,
                column: "Fecha",
                value: new DateTime(2026, 9, 11, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797));

            migrationBuilder.UpdateData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 4,
                column: "Fecha",
                value: new DateTime(2026, 9, 11, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 12, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 12, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 12, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 12, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797));
        }
    }
}
