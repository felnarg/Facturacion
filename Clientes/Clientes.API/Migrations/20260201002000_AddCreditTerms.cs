using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clientes.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditTerms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ApprovedCreditLimit",
                table: "Customers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedPaymentTermDays",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedCreditLimit",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ApprovedPaymentTermDays",
                table: "Customers");
        }
    }
}
