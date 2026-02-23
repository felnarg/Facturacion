using FacturacionElectronica.Domain.Entities;
using FacturacionElectronica.Domain.Enums;
using FacturacionElectronica.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FacturacionElectronica.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context, ILogger? logger = null)
        {
            try
            {
                var tieneEmisores = await context.Emisores.AnyAsync();
                LogMessage(logger, $"Seed: Verificando datos... Emisores: {tieneEmisores}");

                if (tieneEmisores)
                {
                    LogMessage(logger, "Seed: Ya existen datos. Saltando seed.");
                    return;
                }

                LogMessage(logger, "Seed: Iniciando carga de datos de prueba...");

                // ── Emisor ──────────────────────────────────────────────────────────
                var direccionEmisor = new Direccion(
                    calle: "Carrera 10",
                    numero: "25-30",
                    ciudad: "Bogotá",
                    departamento: "Bogotá D.C.",
                    pais: "Colombia"
                ).WithComplemento("Edificio Principal").WithCodigoPostal("110011");

                var contactoEmisor = new InformacionContacto(
                    telefono: "6011234567",
                    email: "contacto@empresadeprueba.com"
                ).WithPaginaWeb("www.empresadeprueba.com");

                var emisor = new Emisor(
                    nit: "9001234567",
                    razonSocial: "EMPRESA DE PRUEBA SAS",
                    tipoPersona: TipoPersona.PersonaJuridica,
                    responsabilidadFiscal: TipoResponsabilidadFiscal.ResponsableIVA,
                    direccion: direccionEmisor,
                    contacto: contactoEmisor,
                    resolucionHabilitacion: "RES-2026-001234",
                    fechaHabilitacion: DateTime.UtcNow.Date,
                    softwareId: "SW123456789",
                    pinSoftware: "PIN123456"
                );

                emisor.ActualizarInformacion(
                    razonSocial: "EMPRESA DE PRUEBA SAS",
                    nombreComercial: "EPRUEBA",
                    direccion: direccionEmisor,
                    contacto: contactoEmisor,
                    registroMercantil: "123456-1"
                );

                // CertificadoDigital y ClaveCertificado son NOT NULL en BD
                emisor.ActualizarCertificado("CERTIFICADO_PENDIENTE", "CLAVE_PENDIENTE");

                await context.Emisores.AddAsync(emisor);
                LogMessage(logger, "Seed: Emisor preparado.");

                // ── Cliente Empresa ─────────────────────────────────────────────────
                var dirClienteEmpresa = new Direccion(
                    calle: "Calle 100",
                    numero: "15-20",
                    ciudad: "Bogotá",
                    departamento: "Bogotá D.C.",
                    pais: "Colombia"
                ).WithComplemento("").WithCodigoPostal("110111");

                var contactoClienteEmpresa = new InformacionContacto(
                    telefono: "6019876543",
                    email: "compras@clienteempresarial.com"
                ).WithPaginaWeb("www.clienteempresarial.com");

                var clienteEmpresa = new Cliente(
                    identificacion: "8001234567",
                    razonSocial: "CLIENTE EMPRESARIAL SAS",
                    tipoPersona: TipoPersona.PersonaJuridica,
                    responsabilidadFiscal: TipoResponsabilidadFiscal.ResponsableIVA,
                    direccion: dirClienteEmpresa,
                    contacto: contactoClienteEmpresa
                );

                clienteEmpresa.ActualizarInformacion(
                    razonSocial: "CLIENTE EMPRESARIAL SAS",
                    nombreComercial: "CLIENTE EMPRESARIAL SAS",
                    responsabilidadFiscal: TipoResponsabilidadFiscal.ResponsableIVA,
                    direccion: dirClienteEmpresa,
                    contacto: contactoClienteEmpresa,
                    registroMercantil: "123456"
                );

                clienteEmpresa.ActualizarUbicacion(
                    codigoPostal: "110111",
                    codigoCiudad: "11001",
                    codigoDepartamento: "11",
                    codigoPais: "CO"
                );

                // ── Cliente Consumidor Final ────────────────────────────────────────
                var dirConsumidor = new Direccion(
                    calle: "Carrera 15",
                    numero: "45-50",
                    ciudad: "Bogotá",
                    departamento: "Bogotá D.C.",
                    pais: "Colombia"
                ).WithComplemento("").WithCodigoPostal("110111");

                var contactoConsumidor = new InformacionContacto(
                    telefono: "6015555555",
                    email: "consumidor@ejemplo.com"
                ).WithPaginaWeb("");

                var clienteConsumidorFinal = new Cliente(
                    identificacion: "222222222222",
                    razonSocial: "CONSUMIDOR FINAL",
                    tipoPersona: TipoPersona.PersonaNatural,
                    responsabilidadFiscal: TipoResponsabilidadFiscal.NoResponsableIVA,
                    direccion: dirConsumidor,
                    contacto: contactoConsumidor
                );

                clienteConsumidorFinal.ActualizarInformacion(
                    razonSocial: "CONSUMIDOR FINAL",
                    nombreComercial: "CONSUMIDOR FINAL",
                    responsabilidadFiscal: TipoResponsabilidadFiscal.NoResponsableIVA,
                    direccion: dirConsumidor,
                    contacto: contactoConsumidor,
                    registroMercantil: ""
                );

                clienteConsumidorFinal.ActualizarUbicacion(
                    codigoPostal: "110111",
                    codigoCiudad: "11001",
                    codigoDepartamento: "11",
                    codigoPais: "CO"
                );

                await context.Clientes.AddRangeAsync(clienteEmpresa, clienteConsumidorFinal);
                LogMessage(logger, "Seed: Clientes preparados.");

                // Guardar Emisores y Clientes antes de consultar el EmisorId
                await context.SaveChangesAsync();
                LogMessage(logger, "Seed: Emisores y Clientes guardados.");

                // ── Numeraciones ───────────────────────────────────────────────────
                var emisorGuardado = await context.Emisores.OrderBy(e => e.Codigo).FirstAsync();
                var fechaInicio = DateTime.UtcNow.Date;
                var fechaFin = fechaInicio.AddYears(1);

                await context.NumeracionesDocumentos.AddRangeAsync(
                    new NumeracionDocumento(
                        prefijo: "SETP",
                        numeroInicial: 1,
                        numeroFinal: 100000,
                        fechaAutorizacion: fechaInicio,
                        fechaVencimiento: fechaFin,
                        tipoDocumento: TipoDocumento.FacturaElectronica,
                        resolucionAutorizacion: "RES-2026-001234-FACT",
                        emisorId: emisorGuardado.Codigo
                    ),
                    new NumeracionDocumento(
                        prefijo: "SETP",
                        numeroInicial: 1,
                        numeroFinal: 50000,
                        fechaAutorizacion: fechaInicio,
                        fechaVencimiento: fechaFin,
                        tipoDocumento: TipoDocumento.NotaCredito,
                        resolucionAutorizacion: "RES-2026-001234-NC",
                        emisorId: emisorGuardado.Codigo
                    ),
                    new NumeracionDocumento(
                        prefijo: "SETP",
                        numeroInicial: 1,
                        numeroFinal: 50000,
                        fechaAutorizacion: fechaInicio,
                        fechaVencimiento: fechaFin,
                        tipoDocumento: TipoDocumento.NotaDebito,
                        resolucionAutorizacion: "RES-2026-001234-ND",
                        emisorId: emisorGuardado.Codigo
                    )
                );

                var cambios = await context.SaveChangesAsync();
                LogMessage(logger, $"Seed: {cambios} cambios finales guardados. Seed completado exitosamente.");
            }
            catch (Exception ex)
            {
                LogError(logger, $"ERROR en Seed: {ex.Message}");
                if (ex.InnerException != null)
                    LogError(logger, $"Inner: {ex.InnerException.Message}");
                throw;
            }
        }

        private static void LogMessage(ILogger? logger, string message)
        {
            if (logger != null) logger.LogInformation(message);
            else Console.WriteLine(message);
        }

        private static void LogError(ILogger? logger, string message)
        {
            if (logger != null) logger.LogError(message);
            else Console.WriteLine(message);
        }
    }
}