using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacturacionElectronica.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RazonSocial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NombreComercial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TipoPersona = table.Column<int>(type: "int", nullable: false),
                    ResponsabilidadFiscal = table.Column<int>(type: "int", nullable: false),
                    Direccion_Calle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion_Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Direccion_Complemento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion_Ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion_Departamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion_CodigoPostal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Direccion_Pais = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Contacto_Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Contacto_Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contacto_PaginaWeb = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RegistroMercantil = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodigoPostal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CodigoCiudad = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CodigoDepartamento = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CodigoPais = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Identificacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.UniqueConstraint("AK_Clientes_Identificacion", x => x.Identificacion);
                });

            migrationBuilder.CreateTable(
                name: "Emisores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RazonSocial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NombreComercial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TipoPersona = table.Column<int>(type: "int", nullable: false),
                    ResponsabilidadFiscal = table.Column<int>(type: "int", nullable: false),
                    Direccion_Calle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion_Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Direccion_Complemento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion_Ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion_Departamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion_CodigoPostal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Direccion_Pais = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Contacto_Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Contacto_Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contacto_PaginaWeb = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RegistroMercantil = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResolucionHabilitacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaHabilitacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoftwareId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PinSoftware = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CertificadoDigital = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClaveCertificado = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Nit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emisores", x => x.Id);
                    table.UniqueConstraint("AK_Emisores_Nit", x => x.Nit);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosElectronicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TipoDocumento = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Cufe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    QrCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    XmlContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    XmlFirmado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RespuestaDian = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaTransmision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Subtotal_Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal_Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TotalDescuentos_Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDescuentos_Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TotalImpuestos_Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalImpuestos_Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Total_Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total_Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TotalPagado_Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPagado_Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SaldoPendiente_Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoPendiente_Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    EmisorId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    ClienteId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosElectronicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentosElectronicos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Identificacion",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentosElectronicos_Emisores_EmisorId",
                        column: x => x.EmisorId,
                        principalTable: "Emisores",
                        principalColumn: "Nit",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NumeracionesDocumentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Prefijo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NumeroInicial = table.Column<long>(type: "bigint", nullable: false),
                    NumeroFinal = table.Column<long>(type: "bigint", nullable: false),
                    NumeroActual = table.Column<long>(type: "bigint", nullable: false),
                    FechaAutorizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoDocumento = table.Column<int>(type: "int", nullable: false),
                    ResolucionAutorizacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmisorId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumeracionesDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NumeracionesDocumentos_Emisores_EmisorId",
                        column: x => x.EmisorId,
                        principalTable: "Emisores",
                        principalColumn: "Nit",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventosDocumentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoEvento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Detalles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DocumentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventosDocumentos_DocumentosElectronicos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "DocumentosElectronicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImpuestosDocumentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoImpuesto = table.Column<int>(type: "int", nullable: false),
                    Porcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    BaseGravable_Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BaseGravable_Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Valor_Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Valor_Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CodigoTributo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NombreTributo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DocumentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpuestosDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImpuestosDocumentos_DocumentosElectronicos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "DocumentosElectronicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsDocumentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CodigoEstandar = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    UnidadMedida = table.Column<int>(type: "int", nullable: false),
                    ValorUnitario_Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorUnitario_Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ValorTotal_Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorTotal_Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Descuento_Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Descuento_Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DocumentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsDocumentos_DocumentosElectronicos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "DocumentosElectronicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PagosDocumentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Valor_Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Valor_Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DocumentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosDocumentos_DocumentosElectronicos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "DocumentosElectronicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Identificacion",
                table: "Clientes",
                column: "Identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosElectronicos_ClienteId_FechaEmision",
                table: "DocumentosElectronicos",
                columns: new[] { "ClienteId", "FechaEmision" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosElectronicos_EmisorId_FechaEmision",
                table: "DocumentosElectronicos",
                columns: new[] { "EmisorId", "FechaEmision" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosElectronicos_Estado",
                table: "DocumentosElectronicos",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosElectronicos_FechaEmision",
                table: "DocumentosElectronicos",
                column: "FechaEmision");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosElectronicos_NumeroDocumento",
                table: "DocumentosElectronicos",
                column: "NumeroDocumento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Emisores_Nit",
                table: "Emisores",
                column: "Nit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventosDocumentos_DocumentoId",
                table: "EventosDocumentos",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImpuestosDocumentos_DocumentoId",
                table: "ImpuestosDocumentos",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsDocumentos_DocumentoId",
                table: "ItemsDocumentos",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_NumeracionesDocumentos_EmisorId_TipoDocumento_Activo",
                table: "NumeracionesDocumentos",
                columns: new[] { "EmisorId", "TipoDocumento", "Activo" },
                filter: "[Activo] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_PagosDocumentos_DocumentoId",
                table: "PagosDocumentos",
                column: "DocumentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventosDocumentos");

            migrationBuilder.DropTable(
                name: "ImpuestosDocumentos");

            migrationBuilder.DropTable(
                name: "ItemsDocumentos");

            migrationBuilder.DropTable(
                name: "NumeracionesDocumentos");

            migrationBuilder.DropTable(
                name: "PagosDocumentos");

            migrationBuilder.DropTable(
                name: "DocumentosElectronicos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Emisores");
        }
    }
}
