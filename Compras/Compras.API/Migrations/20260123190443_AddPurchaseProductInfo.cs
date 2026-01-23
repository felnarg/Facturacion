using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Compras.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseProductInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InternalProductCode",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "Purchases",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalProductCode",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "Purchases");
        }
    }
}
