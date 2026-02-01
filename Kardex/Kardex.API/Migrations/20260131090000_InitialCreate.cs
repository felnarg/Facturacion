using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kardex.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IdentificationType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IdentificationNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentTermDays = table.Column<int>(type: "int", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreditAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SaleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditMovements_CreditAccounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "CreditAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CreditAccounts",
                columns: new[]
                {
                    "Id", "CustomerName", "IdentificationType", "IdentificationNumber",
                    "CreditLimit", "PaymentTermDays", "CurrentBalance", "CreatedAt", "UpdatedAt"
                },
                values: new object[,]
                {
                    {
                        new Guid("b10efb7e-1d4a-4b55-9f45-1f3f9b42a111"),
                        "Comercializadora La 45",
                        "NIT",
                        "900123456-7",
                        5000000m,
                        30,
                        0m,
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        new Guid("a2ff7e2b-c7f2-4a1b-b0b7-7771e3a2b222"),
                        "Carlos Medina",
                        "CC",
                        "1030123456",
                        1200000m,
                        20,
                        0m,
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditAccounts_IdentificationType_IdentificationNumber",
                table: "CreditAccounts",
                columns: new[] { "IdentificationType", "IdentificationNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditMovements_CreditAccountId",
                table: "CreditMovements",
                column: "CreditAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditMovements");

            migrationBuilder.DropTable(
                name: "CreditAccounts");
        }
    }
}
