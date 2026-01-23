using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalogo.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProductCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InternalProductCode",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SupplierProductCode",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("2b6d7b3f-7c3c-4f6d-a9f5-7c5b5b9e0f01"),
                columns: new[] { "InternalProductCode", "SupplierProductCode" },
                values: new object[] { 50001, 11001 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("9a2e8f9a-6d6d-4b6a-9c5f-1f8b6f3e4e02"),
                columns: new[] { "InternalProductCode", "SupplierProductCode" },
                values: new object[] { 50002, 11002 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalProductCode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SupplierProductCode",
                table: "Products");
        }
    }
}
