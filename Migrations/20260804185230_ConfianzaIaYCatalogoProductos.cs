using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacturasIA.Platform.Migrations
{
    /// <inheritdoc />
    public partial class ConfianzaIaYCatalogoProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductoId",
                table: "item_facturas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "ItemsRequierenRevision",
                table: "facturas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "confianza_campos",
                table: "facturas",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProveedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_productos_ProveedorId_Nombre",
                table: "productos",
                columns: new[] { "ProveedorId", "Nombre" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "productos");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "item_facturas");

            migrationBuilder.DropColumn(
                name: "ItemsRequierenRevision",
                table: "facturas");

            migrationBuilder.DropColumn(
                name: "confianza_campos",
                table: "facturas");
        }
    }
}
