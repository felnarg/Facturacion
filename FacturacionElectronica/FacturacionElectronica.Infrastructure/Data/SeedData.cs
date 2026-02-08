using FacturacionElectronica.Domain.Entities;
using FacturacionElectronica.Domain.Enums;
using FacturacionElectronica.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FacturacionElectronica.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            // Verificar si ya hay datos
            if (await context.Emisores.AnyAsync())
                return;

            await SeedEmisores(context);
            await SeedClientes(context);
            await SeedNumeraciones(context);
            
            await context.SaveChangesAsync();
        }

        private static async Task SeedEmisores(ApplicationDbContext context)
        {
            var emisor = new Emisor(
                nit: "9001234567",
                razonSocial: "EMPRESA DE PRUEBA SAS",
                tipoPersona: TipoPersona.PersonaJuridica,
                responsabilidadFiscal: TipoResponsabilidadFiscal.ResponsableIVA,
                direccion: new Direccion(
                    calle: "Carrera 10",
                    numero: "25-30",
                    ciudad: "Bogotá",
                    departamento: "Bogotá D.C.",
                    pais: "Colombia"
                ).WithComplemento("Edificio Principal"),
                contacto: new InformacionContacto(
                    telefono: "6011234567",
                    email: "contacto@empresadeprueba.com"
                ).WithPaginaWeb("www.empresadeprueba.com"),
                resolucionHabilitacion: "RES-2024-001234",
                fechaHabilitacion: new DateTime(2024, 1, 1),
                softwareId: "SW123456789",
                pinSoftware: "PIN123456"
            );

            emisor.ActualizarInformacion(
                razonSocial: "EMPRESA DE PRUEBA SAS",
                nombreComercial: "EPRUEBA",
                direccion: new Direccion(
                    calle: "Carrera 10",
                    numero: "25-30",
                    ciudad: "Bogotá",
                    departamento: "Bogotá D.C.",
                    pais: "Colombia"
                ).WithComplemento("Edificio Principal"),
                contacto: new InformacionContacto(
                    telefono: "6011234567",
                    email: "contacto@empresadeprueba.com"
                ).WithPaginaWeb("www.empresadeprueba.com"),
                registroMercantil: "123456-1"
            );

            await context.Emisores.AddAsync(emisor);
        }

        private static async Task SeedClientes(ApplicationDbContext context)
        {
            // Cliente empresa
            var clienteEmpresa = new Cliente(
                identificacion: "8001234567",
                razonSocial: "CLIENTE EMPRESARIAL SAS",
                tipoPersona: TipoPersona.PersonaJuridica,
                responsabilidadFiscal: TipoResponsabilidadFiscal.ResponsableIVA,
                direccion: new Direccion(
                    calle: "Calle 100",
                    numero: "15-20",
                    ciudad: "Bogotá",
                    departamento: "Bogotá D.C.",
                    pais: "Colombia"
                ),
                contacto: new InformacionContacto(
                    telefono: "6019876543",
                    email: "compras@clienteempresarial.com"
                )
            );

            clienteEmpresa.ActualizarUbicacion(
                codigoPostal: "110111",
                codigoCiudad: "11001",
                codigoDepartamento: "11",
                codigoPais: "CO"
            );

            // Cliente consumidor final
            var clienteConsumidorFinal = new Cliente(
                identificacion: "222222222222",
                razonSocial: "CONSUMIDOR FINAL",
                tipoPersona: TipoPersona.PersonaNatural,
                responsabilidadFiscal: TipoResponsabilidadFiscal.NoResponsableIVA,
                direccion: new Direccion(
                    calle: "Carrera 15",
                    numero: "45-50",
                    ciudad: "Bogotá",
                    departamento: "Bogotá D.C.",
                    pais: "Colombia"
                ),
                contacto: new InformacionContacto(
                    telefono: "6015555555",
                    email: "consumidor@ejemplo.com"
                )
            );

            await context.Clientes.AddRangeAsync(clienteEmpresa, clienteConsumidorFinal);
        }

        private static async Task SeedNumeraciones(ApplicationDbContext context)
        {
            var emisor = await context.Emisores.FirstAsync();
            
            var numeracionFactura = new NumeracionDocumento(
                prefijo: "SETP",
                numeroInicial: 1,
                numeroFinal: 100000,
                fechaAutorizacion: new DateTime(2024, 1, 1),
                fechaVencimiento: new DateTime(2024, 12, 31),
                tipoDocumento: TipoDocumento.FacturaElectronica,
                resolucionAutorizacion: "RES-2024-001234-FACT",
                emisorId: emisor.Codigo
            );

            var numeracionNotaCredito = new NumeracionDocumento(
                prefijo: "SETP",
                numeroInicial: 1,
                numeroFinal: 50000,
                fechaAutorizacion: new DateTime(2024, 1, 1),
                fechaVencimiento: new DateTime(2024, 12, 31),
                tipoDocumento: TipoDocumento.NotaCredito,
                resolucionAutorizacion: "RES-2024-001234-NC",
                emisorId: emisor.Codigo
            );

            var numeracionNotaDebito = new NumeracionDocumento(
                prefijo: "SETP",
                numeroInicial: 1,
                numeroFinal: 50000,
                fechaAutorizacion: new DateTime(2024, 1, 1),
                fechaVencimiento: new DateTime(2024, 12, 31),
                tipoDocumento: TipoDocumento.NotaDebito,
                resolucionAutorizacion: "RES-2024-001234-ND",
                emisorId: emisor.Codigo
            );

            await context.NumeracionesDocumentos.AddRangeAsync(
                numeracionFactura,
                numeracionNotaCredito,
                numeracionNotaDebito
            );
        }
    }
}