using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalogo.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProductIvaRemoveSku : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sku",
                table: "Products");

            migrationBuilder.AddColumn<decimal>(
                name: "ConsumptionTaxPercentage",
                table: "Products",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Iva",
                table: "Products",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 19m);

            migrationBuilder.AddColumn<decimal>(
                name: "SpecialSalePercentage",
                table: "Products",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 20m);

            migrationBuilder.AddColumn<decimal>(
                name: "WholesaleSalePercentage",
                table: "Products",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 25m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("2b6d7b3f-7c3c-4f6d-a9f5-7c5b5b9e0f01"),
                columns: new[] { "Iva", "SpecialSalePercentage", "WholesaleSalePercentage" },
                values: new object[] { 19m, 20m, 25m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("9a2e8f9a-6d6d-4b6a-9c5f-1f8b6f3e4e02"),
                columns: new[] { "Iva", "SpecialSalePercentage", "WholesaleSalePercentage" },
                values: new object[] { 19m, 20m, 25m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsumptionTaxPercentage",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Iva",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SpecialSalePercentage",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WholesaleSalePercentage",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("2b6d7b3f-7c3c-4f6d-a9f5-7c5b5b9e0f01"),
                column: "Sku",
                value: "SKU-BASE-001");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("9a2e8f9a-6d6d-4b6a-9c5f-1f8b6f3e4e02"),
                column: "Sku",
                value: "SKU-PREM-002");
        }
    }
}
