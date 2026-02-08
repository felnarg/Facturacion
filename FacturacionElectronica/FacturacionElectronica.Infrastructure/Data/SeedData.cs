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
            try
            {
                // Verificar si ya hay datos
                var tieneEmisores = await context.Emisores.AnyAsync();
                if (tieneEmisores)
                {
                    Console.WriteLine("Seed: Ya existen emisores en la base de datos. Saltando seed.");
                    return;
                }

                Console.WriteLine("Seed: Iniciando carga de datos de prueba...");

                await SeedEmisores(context);
                Console.WriteLine("Seed: Emisores creados correctamente.");

                await SeedClientes(context);
                Console.WriteLine("Seed: Clientes creados correctamente.");

                await SeedNumeraciones(context);
                Console.WriteLine("Seed: Numeraciones creadas correctamente.");
                
                var cambios = await context.SaveChangesAsync();
                Console.WriteLine($"Seed: {cambios} cambios guardados exitosamente.");
                Console.WriteLine("Seed: Datos de prueba cargados correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en Seed: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw; // Re-lanzar para que se vea en los logs
            }
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
                resolucionHabilitacion: "RES-2026-001234",
                fechaHabilitacion: DateTime.UtcNow.Date,
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
            
            // Usar fechas actuales para que las numeraciones estén vigentes
            var fechaInicio = DateTime.UtcNow.Date;
            var fechaFin = fechaInicio.AddYears(1);

            var numeracionFactura = new NumeracionDocumento(
                prefijo: "SETP",
                numeroInicial: 1,
                numeroFinal: 100000,
                fechaAutorizacion: fechaInicio,
                fechaVencimiento: fechaFin,
                tipoDocumento: TipoDocumento.FacturaElectronica,
                resolucionAutorizacion: "RES-2026-001234-FACT",
                emisorId: emisor.Codigo
            );

            var numeracionNotaCredito = new NumeracionDocumento(
                prefijo: "SETP",
                numeroInicial: 1,
                numeroFinal: 50000,
                fechaAutorizacion: fechaInicio,
                fechaVencimiento: fechaFin,
                tipoDocumento: TipoDocumento.NotaCredito,
                resolucionAutorizacion: "RES-2026-001234-NC",
                emisorId: emisor.Codigo
            );

            var numeracionNotaDebito = new NumeracionDocumento(
                prefijo: "SETP",
                numeroInicial: 1,
                numeroFinal: 50000,
                fechaAutorizacion: fechaInicio,
                fechaVencimiento: fechaFin,
                tipoDocumento: TipoDocumento.NotaDebito,
                resolucionAutorizacion: "RES-2026-001234-ND",
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