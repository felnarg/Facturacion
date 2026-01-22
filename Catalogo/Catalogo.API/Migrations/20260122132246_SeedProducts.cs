using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Catalogo.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Price", "Sku", "Stock", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2b6d7b3f-7c3c-4f6d-a9f5-7c5b5b9e0f01"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Producto de ejemplo para el catalogo", "Producto base", 19.99m, "SKU-BASE-001", 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("9a2e8f9a-6d6d-4b6a-9c5f-1f8b6f3e4e02"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Producto premium de ejemplo", "Producto premium", 79.99m, "SKU-PREM-002", 50, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("2b6d7b3f-7c3c-4f6d-a9f5-7c5b5b9e0f01"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("9a2e8f9a-6d6d-4b6a-9c5f-1f8b6f3e4e02"));
        }
    }
}
