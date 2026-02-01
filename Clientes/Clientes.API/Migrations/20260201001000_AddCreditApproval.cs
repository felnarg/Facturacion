using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clientes.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCreditApproved",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCreditApproved",
                table: "Customers");
        }
    }
}
