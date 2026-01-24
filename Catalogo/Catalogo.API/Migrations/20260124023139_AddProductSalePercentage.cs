using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalogo.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSalePercentage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SalePercentage",
                table: "Products",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 30m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("2b6d7b3f-7c3c-4f6d-a9f5-7c5b5b9e0f01"),
                column: "SalePercentage",
                value: 30m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("9a2e8f9a-6d6d-4b6a-9c5f-1f8b6f3e4e02"),
                column: "SalePercentage",
                value: 25m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalePercentage",
                table: "Products");
        }
    }
}
