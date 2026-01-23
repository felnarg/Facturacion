using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Compras.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSuppliers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "ContactName", "CreatedAt", "Email", "Name", "Phone" },
                values: new object[,]
                {
                    { new Guid("3b99c56f-9a9a-4f2f-8c4c-8f31b3c3c333"), "Av. 80 # 40-30", "Luisa Gomez", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "luisa@alimentos-premium.com", "Alimentos Premium", "+57 300 333 4455" },
                    { new Guid("7b91c222-62cb-4e1b-8c1b-0f3cf01a1a11"), "Calle 10 # 15-20", "Maria Lopez", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "maria@distribuidoracentral.com", "Distribuidora Central", "+57 300 111 2233" },
                    { new Guid("e12f64b9-0b0b-4f7f-9b2b-9e9d4d2a2a22"), "Carrera 45 # 22-10", "Juan Perez", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "juan@proveedoresnorte.com", "Proveedores del Norte", "+57 300 222 3344" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}
