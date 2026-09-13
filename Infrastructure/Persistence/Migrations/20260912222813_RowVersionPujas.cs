using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RowVersionPujas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Pujas",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

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
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 12, 22, 53, 12, 785, DateTimeKind.Utc).AddTicks(797), new DateTime(2026, 9, 12, 21, 28, 12, 785, DateTimeKind.Utc).AddTicks(797) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 12, 22, 29, 12, 785, DateTimeKind.Utc).AddTicks(797), new DateTime(2026, 9, 12, 21, 28, 12, 785, DateTimeKind.Utc).AddTicks(797) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 14, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797), new DateTime(2026, 9, 13, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 11, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797), new DateTime(2026, 9, 10, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 11, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797), new DateTime(2026, 9, 10, 22, 28, 12, 785, DateTimeKind.Utc).AddTicks(797) });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Pujas");

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaPuja",
                value: new DateTime(2026, 9, 7, 21, 40, 24, 987, DateTimeKind.Utc).AddTicks(7940));

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaPuja",
                value: new DateTime(2026, 9, 7, 21, 55, 24, 987, DateTimeKind.Utc).AddTicks(7940));

            migrationBuilder.UpdateData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaPuja",
                value: new DateTime(2026, 9, 6, 21, 50, 24, 987, DateTimeKind.Utc).AddTicks(7940));

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 7, 22, 25, 24, 987, DateTimeKind.Utc).AddTicks(7940), new DateTime(2026, 9, 7, 21, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 7, 22, 1, 24, 987, DateTimeKind.Utc).AddTicks(7940), new DateTime(2026, 9, 7, 21, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 9, 22, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940), new DateTime(2026, 9, 8, 22, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 6, 22, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940), new DateTime(2026, 9, 5, 22, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 6, 22, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940), new DateTime(2026, 9, 5, 22, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940) });

            migrationBuilder.UpdateData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 1,
                column: "Fecha",
                value: new DateTime(2026, 9, 6, 22, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940));

            migrationBuilder.UpdateData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 2,
                column: "Fecha",
                value: new DateTime(2026, 9, 7, 21, 55, 24, 987, DateTimeKind.Utc).AddTicks(7940));

            migrationBuilder.UpdateData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 3,
                column: "Fecha",
                value: new DateTime(2026, 9, 6, 22, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940));

            migrationBuilder.UpdateData(
                table: "TransaccionesLedger",
                keyColumn: "Id",
                keyValue: 4,
                column: "Fecha",
                value: new DateTime(2026, 9, 6, 22, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 7, 22, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 7, 22, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 7, 22, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 7, 22, 0, 24, 987, DateTimeKind.Utc).AddTicks(7940));
        }
    }
}
