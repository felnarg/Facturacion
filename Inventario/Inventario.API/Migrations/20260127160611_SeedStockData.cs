using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Inventario.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedStockData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "StockMovements",
                columns: new[] { "Id", "CreatedAt", "ProductId", "Quantity", "ReferenceId", "Type" },
                values: new object[,]
                {
                    { new Guid("a89b7c6d-4444-4444-8888-1f8b6f3e4e02"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("9a2e8f9a-6d6d-4b6a-9c5f-1f8b6f3e4e02"), 50, null, "Creation" },
                    { new Guid("f67a8b9c-3333-4444-8888-c75b5b9e0f01"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2b6d7b3f-7c3c-4f6d-a9f5-7c5b5b9e0f01"), 100, null, "Creation" }
                });

            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "CreatedAt", "ProductId", "Quantity", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("d34e9a8b-1111-4444-8888-c75b5b9e0f01"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2b6d7b3f-7c3c-4f6d-a9f5-7c5b5b9e0f01"), 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e56f8b9c-2222-4444-8888-1f8b6f3e4e02"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("9a2e8f9a-6d6d-4b6a-9c5f-1f8b6f3e4e02"), 50, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: new Guid("a89b7c6d-4444-4444-8888-1f8b6f3e4e02"));

            migrationBuilder.DeleteData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: new Guid("f67a8b9c-3333-4444-8888-c75b5b9e0f01"));

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: new Guid("d34e9a8b-1111-4444-8888-c75b5b9e0f01"));

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: new Guid("e56f8b9c-2222-4444-8888-1f8b6f3e4e02"));
        }
    }
}
