using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Compras.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierInvoiceNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupplierInvoiceNumber",
                table: "Purchases",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierInvoiceNumber",
                table: "Purchases");
        }
    }
}
