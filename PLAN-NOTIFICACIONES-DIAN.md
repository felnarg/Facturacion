# 📋 Plan de Trabajo: SOAP DIAN + Microservicio de Notificaciones

> **Fecha:** 2026-02-23  
> **Estado:** Pendiente de aprobación  
> **Scope:** Dos fases en paralelo sobre la base existente del sistema de facturación electrónica

---

## 🗺️ Visión General del Flujo Final

```
[POST /api/documentoselectronicos]
         │
         ▼
  FacturacionElectronica
  ┌──────────────────────────────────────────────────────────┐
  │  1. Crea DocumentoElectronico (Estado: Pendiente)        │
  │  2. XmlGeneratorService  → genera XML UBL 2.1            │
  │     documento.MarcarComoGenerado(xml)                    │
  │  3. FirmaDigitalService  → firma XML con cert .p12       │
  │     documento.MarcarComoFirmado(xmlFirmado, cufe)        │
  │  4. DianSoapService      → llama WS DIAN habilitación    │
  │     documento.MarcarComoTransmitido(fecha)               │
  │     documento.ProcesarRespuestaDian(resp, aceptado)      │
  │  5. Publica evento RabbitMQ: FacturaTransmitidaDian      │
  └──────────────────────────────────────────────────────────┘
         │
         │ [factura.transmitida.dian] → RabbitMQ
         │
         ▼
  Notificaciones
  ┌──────────────────────────────────────────────────────────┐
  │  Consume evento FacturaTransmitidaDian                   │
  │  ├── Email HTML al cliente (adjunto: resumen factura)    │
  │  └── Registra auditoría de notificación en BD            │
  └──────────────────────────────────────────────────────────┘
```

---

## 📦 FASE 1 — Servicios SOAP/XML/Firma en `FacturacionElectronica`

### Paso 1.1 — Agregar NuGet packages a Infrastructure

**Archivo:** `FacturacionElectronica.Infrastructure.csproj`

Paquetes a agregar:
- `System.ServiceModel.Http` (cliente WCF/SOAP)
- `BouncyCastle.NetCore` (firma digital XML)
- `System.Security.Cryptography.Xml` (XmlDSig)

---

### Paso 1.2 — Crear interfaces de dominio para los servicios

**Archivo:** `FacturacionElectronica.Domain/Interfaces/IXmlGeneratorService.cs`
```csharp
public interface IXmlGeneratorService
{
    Task<string> GenerarXmlUbl21Async(DocumentoElectronico documento, Emisor emisor, Cliente cliente);
}
```

**Archivo:** `FacturacionElectronica.Domain/Interfaces/IFirmaDigitalService.cs`
```csharp
public interface IFirmaDigitalService
{
    Task<(string XmlFirmado, string Cufe)> FirmarDocumentoAsync(string xmlContent, Emisor emisor);
}
```

**Archivo:** `FacturacionElectronica.Domain/Interfaces/IDianSoapService.cs`
```csharp
public interface IDianSoapService
{
    Task<RespuestaDian> TransmitirDocumentoAsync(string xmlFirmado, Emisor emisor);
}
```

**Archivo:** `FacturacionElectronica.Domain/ValueObjects/RespuestaDian.cs`
```csharp
public record RespuestaDian(
    bool Aceptado,
    string Mensaje,
    string ZipKey,       // Clave del ZIP enviado a la DIAN
    string TrackId,      // ID de rastreo de la DIAN
    DateTime FechaRespuesta
);
```

---

### Paso 1.3 — Implementar `XmlGeneratorService`

**Archivo:** `FacturacionElectronica.Infrastructure/Services/XmlGeneratorService.cs`

Genera el XML UBL 2.1 según el estándar colombiano DIAN:
- Namespace: `urn:oasis:names:specification:ubl:schema:xsd:Invoice-2`
- Incluye: emisor, receptor, líneas de factura, impuestos, totales
- Formato de fecha: `yyyy-MM-dd`
- Moneda: `COP`

---

### Paso 1.4 — Implementar `FirmaDigitalService`

**Archivo:** `FacturacionElectronica.Infrastructure/Services/FirmaDigitalService.cs`

Responsabilidades:
- Carga el certificado `.p12` del `Emisor.CertificadoDigital` (Base64)
- Firma el XML con `XmlDSig` (RSA-SHA256)
- Calcula el **CUFE** (Código Único de Factura Electrónica):
  ```
  CUFE = SHA384(NumFac + FecFac + HorFac + ValFac + NitOFE + NumAdq + ClTec)
  ```
- En modo prueba: usa `ClTec` del ambiente de habilitación DIAN

---

### Paso 1.5 — Implementar `DianSoapService`

**Archivo:** `FacturacionElectronica.Infrastructure/Services/DianSoapService.cs`

Responsabilidades:
- Empaqueta el XML firmado en un ZIP (requerimiento DIAN)
- Convierte el ZIP a Base64
- Llama al WebService DIAN:
  - **Pruebas:** `https://vpfe-hab.dian.gov.co/WcfDianCustomerServices.svc`
  - **Producción:** `https://vpfe.dian.gov.co/WcfDianCustomerServices.svc`
- Método SOAP: `SendTestSetAsync` (pruebas) / `SendBillAsync` (producción)
- Parsea la respuesta XML de la DIAN

---

### Paso 1.6 — Crear `IEventBus` como interfaz en Domain/Application

**Archivo:** `FacturacionElectronica.Application/Interfaces/IEventBus.cs`
```csharp
public interface IEventBus
{
    void Publish<T>(T @event) where T : IntegrationEvent;
}
```

> Mover la interfaz actual de Infrastructure a Application para respetar la Dependency Rule de Clean Architecture.

---

### Paso 1.7 — Crear evento de integración `FacturaTransmitidaDian`

**Archivo:** `Shared/Facturacion.Shared/Events/FacturaTransmitidaDian.cs`
```csharp
public record FacturaTransmitidaDian(
    Guid DocumentoId,
    string NumeroDocumento,
    string EmisorNit,
    string EmisorNombre,
    string EmisorEmail,
    string ClienteIdentificacion,
    string ClienteNombre,
    string ClienteEmail,
    decimal Total,
    DateTime FechaEmision,
    string Cufe,
    bool Aceptado,
    string RespuestaDian
) : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);
```

---

### Paso 1.8 — Actualizar el Controller para orquestar el flujo completo

**Archivo:** `FacturacionElectronica.API/Controllers/DocumentoElectronicoController.cs`

Agregar endpoints:
- `POST /api/documentoselectronicos` → Crear + Generar XML + Firmar + Transmitir DIAN (flujo completo)
- `POST /api/documentoselectronicos/{id}/transmitir` → Retransmitir un documento existente
- `GET  /api/documentoselectronicos/{id}/xml` → Descargar XML firmado
- `GET  /api/documentoselectronicos/health` → Health check

Agregar configuración `appsettings.json`:
```json
"Dian": {
  "AmbientePruebas": true,
  "UrlServicioHabilitacion": "https://vpfe-hab.dian.gov.co/WcfDianCustomerServices.svc",
  "UrlServicioProduccion": "https://vpfe.dian.gov.co/WcfDianCustomerServices.svc",
  "TestSetId": "PUT-YOUR-TESTSET-ID-HERE"
}
```

---

### Paso 1.9 — Registrar servicios en `Program.cs`

```csharp
builder.Services.AddScoped<IXmlGeneratorService, XmlGeneratorService>();
builder.Services.AddScoped<IFirmaDigitalService, FirmaDigitalService>();
builder.Services.AddScoped<IDianSoapService, DianSoapService>();
builder.Services.AddSingleton<IEventBus>(sp => new RabbitMQEventBus(...));
```

---

## 📦 FASE 2 — Microservicio `Notificaciones`

### Estructura de carpetas a crear

```
Notificaciones/
├── Notificaciones.Domain/
│   ├── Entities/Notificacion.cs
│   ├── Enums/TipoNotificacion.cs
│   ├── Enums/EstadoNotificacion.cs
│   ├── Enums/CanalNotificacion.cs
│   ├── ValueObjects/EmailDestinatario.cs
│   └── Interfaces/
│       ├── INotificacionRepository.cs
│       └── IEmailSender.cs
│
├── Notificaciones.Application/
│   ├── EventHandlers/FacturaTransmitidaDianHandler.cs
│   ├── Services/NotificacionService.cs
│   ├── Templates/FacturaClienteHtmlTemplate.cs
│   └── DTOs/EnviarEmailDto.cs
│
├── Notificaciones.Infrastructure/
│   ├── Email/SmtpEmailSender.cs
│   ├── EventBus/RabbitMQConsumer.cs
│   ├── Data/
│   │   ├── NotificacionesDbContext.cs
│   │   └── Configurations/NotificacionConfiguration.cs
│   └── Repositories/NotificacionRepository.cs
│
└── Notificaciones.API/
    ├── Program.cs
    ├── Workers/EventConsumerWorker.cs
    ├── Controllers/HealthController.cs
    ├── appsettings.json
    └── Dockerfile
```

---

### Paso 2.1 — `Notificaciones.Domain`

**`Notificacion.cs`** — Entidad de auditoría:
```csharp
public class Notificacion : EntityBase
{
    public Guid DocumentoId { get; private set; }
    public string Destinatario { get; private set; }    // email
    public string NombreDestinatario { get; private set; }
    public TipoNotificacion Tipo { get; private set; }
    public CanalNotificacion Canal { get; private set; }
    public EstadoNotificacion Estado { get; private set; }
    public string Asunto { get; private set; }
    public DateTime FechaIntento { get; private set; }
    public DateTime? FechaEnvio { get; private set; }
    public string? ErrorMensaje { get; private set; }
    public int Intentos { get; private set; }
}
```

**Enums:**
- `TipoNotificacion`: `FacturaCliente`, `FacturaAceptadaDian`, `FacturaRechazadaDian`
- `EstadoNotificacion`: `Pendiente`, `Enviada`, `Fallida`, `Reintentando`
- `CanalNotificacion`: `Email`, `SMS` (para futuro)

**`IEmailSender.cs`:**
```csharp
public interface IEmailSender
{
    Task<bool> EnviarAsync(string destinatario, string nombre, string asunto, string htmlBody);
}
```

---

### Paso 2.2 — `Notificaciones.Application`

**`FacturaTransmitidaDianHandler.cs`:**
```csharp
public class FacturaTransmitidaDianHandler
{
    // Consume el evento FacturaTransmitidaDian
    // Llama a NotificacionService para generar y enviar email al cliente
    // Registra la notificación en BD (auditoría)
}
```

**`FacturaClienteHtmlTemplate.cs`:**
- Template HTML responsivo con:
  - Logo (placeholder)
  - Número de factura
  - CUFE (Código Único de Factura Electrónica)
  - Fecha de emisión
  - Tabla de items (si se incluyen en el evento)
  - Total facturado
  - Estado DIAN (Aceptada / Rechazada)
  - Datos del emisor

---

### Paso 2.3 — `Notificaciones.Infrastructure`

**`SmtpEmailSender.cs`** — Implementa `IEmailSender` con **MailKit**:
```csharp
// Configuración por appsettings.json:
// "Email": {
//   "SmtpHost": "mailhog" (docker) | "smtp.gmail.com" (prod),
//   "SmtpPort": 1025 (mailhog) | 587 (prod),
//   "UseSsl": false | true,
//   "Usuario": "",
//   "Password": "",
//   "RemitenteName": "Sistema de Facturación",
//   "RemitenteEmail": "facturacion@empresa.com"
// }
```

**`RabbitMQConsumer.cs`** — Worker que suscribe a:
- Queue: `notificaciones.events`
- Routing key: `factura.transmitida.dian`
- Exchange: `facturacion.events` (el mismo existente)

**`NotificacionesDbContext.cs`** + migración inicial.

---

### Paso 2.4 — `Notificaciones.API`

**`Program.cs`:**
```csharp
// Minimal API: solo health check + swagger
// Registra IEmailSender → SmtpEmailSender
// Registra INotificacionRepository
// Registra hosted service: EventConsumerWorker
// Registra DbContext (SQL Server, base: Notificaciones)
```

**`EventConsumerWorker.cs`** — `BackgroundService`:
```csharp
// Al iniciar: conecta a RabbitMQ
// Loop: consume mensajes de la queue notificaciones.events
// Por cada mensaje: deserializa → llama al Handler → ACK
```

**`HealthController.cs`:**
```csharp
// GET /api/notificaciones/health
// Devuelve: { status: "ok", timestamp, smtpConnected: bool, rabbitConnected: bool }
```

**`Dockerfile`** — mismo patrón que FacturacionElectronica.API

---

### Paso 2.5 — `Notificaciones.Infrastructure.csproj`

Paquetes NuGet:
- `MailKit` (SMTP)
- `MimeKit` (construcción de emails HTML)
- `RabbitMQ.Client`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Serilog.AspNetCore`

---

## 🐳 FASE 3 — Docker y API Gateway

### Paso 3.1 — Agregar MailHog al `docker-compose.yml`

```yaml
mailhog:
  image: mailhog/mailhog
  container_name: supermercado-mailhog
  ports:
    - "1025:1025"   # SMTP
    - "8025:8025"   # UI web para ver emails
  networks:
    - supermercado-network
```

### Paso 3.2 — Agregar `notificaciones-api` al `docker-compose.yml`

```yaml
notificaciones-api:
  image: notificaciones-api-img
  build:
    context: .
    dockerfile: Notificaciones/Notificaciones.API/Dockerfile
  environment:
    - ASPNETCORE_ENVIRONMENT=Production
    - ConnectionStrings__DefaultConnection=Data Source=sqlserver,...
    - RabbitMQ__HostName=rabbitmq
    - RabbitMQ__UserName=admin
    - RabbitMQ__Password=p@ssword123
    - RabbitMQ__Exchange=facturacion.events
    - RabbitMQ__Queue=notificaciones.events
    - Email__SmtpHost=mailhog
    - Email__SmtpPort=1025
    - Email__UseSsl=false
    - Email__RemitenteEmail=facturacion@empresa.com
    - Email__RemitenteName=Sistema de Facturación Electrónica
  ports:
    - "8088:80"
  depends_on:
    - rabbitmq
    - sqlserver
    - mailhog
  networks:
    - supermercado-network
```

### Paso 3.3 — Actualizar `ApiGateway/ApiGateway/appsettings.json`

Agregar ruta:
```json
"notificaciones-route": {
  "ClusterId": "notificaciones-cluster",
  "Match": { "Path": "/api/notificaciones/{**remainder}" }
}
```

Agregar cluster:
```json
"notificaciones-cluster": {
  "Destinations": {
    "destination1": { "Address": "http://notificaciones-api" }
  }
}
```

---

## 🧪 FASE 4 — Guía de Pruebas

### Paso 4.1 — Crear `GUIA-PRUEBAS-DIAN-NOTIFICACIONES.md`

Contenido:
1. **Cómo probar el SOAP DIAN en ambiente de habilitación**
   - URL del portal DIAN habilitación
   - Cómo obtener un TestSetId
   - Qué esperar en la respuesta (ZipKey, TrackId)
   - Cómo verificar en el portal DIAN

2. **Cómo ver los emails en MailHog**
   - Abrir `http://localhost:8025`
   - Identificar el email del cliente
   - Verificar el contenido HTML

3. **Flujo completo de prueba con curl/Postman**
   - Crear documento → ver respuesta DIAN → ver email en MailHog

---

## 📊 Resumen de Archivos

| # | Archivo | Acción |
|---|---------|--------|
| 1 | `FacturacionElectronica.Infrastructure.csproj` | Modificar (+ NuGet) |
| 2 | `Domain/Interfaces/IXmlGeneratorService.cs` | Crear |
| 3 | `Domain/Interfaces/IFirmaDigitalService.cs` | Crear |
| 4 | `Domain/Interfaces/IDianSoapService.cs` | Crear |
| 5 | `Domain/ValueObjects/RespuestaDian.cs` | Crear |
| 6 | `Application/Interfaces/IEventBus.cs` | Crear |
| 7 | `Infrastructure/Services/XmlGeneratorService.cs` | Crear |
| 8 | `Infrastructure/Services/FirmaDigitalService.cs` | Crear |
| 9 | `Infrastructure/Services/DianSoapService.cs` | Crear |
| 10 | `Shared/Events/FacturaTransmitidaDian.cs` | Crear |
| 11 | `FacturacionElectronica.API/Controllers/DocumentoElectronicoController.cs` | Modificar |
| 12 | `FacturacionElectronica.API/Program.cs` | Modificar |
| 13 | `FacturacionElectronica.API/appsettings.json` | Modificar |
| 14 | **18 archivos del microservicio Notificaciones** | Crear |
| 15 | `docker-compose.yml` | Modificar |
| 16 | `ApiGateway/appsettings.json` | Modificar |
| 17 | `GUIA-PRUEBAS-DIAN-NOTIFICACIONES.md` | Crear |

**Total: ~34 archivos**

---

## ⚠️ Consideraciones Importantes

1. **Certificado digital real:** Para producción se necesita un certificado `.p12` emitido por una entidad certificadora autorizada por la DIAN. En desarrollo se usa un certificado auto-firmado de prueba.

2. **TestSetId DIAN:** Para las pruebas de habilitación, la DIAN asigna un `TestSetId` cuando se registra el software en el portal. En el plan se usa un placeholder configurable.

3. **XML UBL 2.1:** La DIAN tiene un XSD (schema) específico para Colombia. Se implementará una versión simplificada compatible con las validaciones básicas de habilitación.

4. **MailHog:** Solo para desarrollo/QA. En producción se configura un SMTP real (Gmail App Password, SendGrid, AWS SES, etc.) via variables de entorno en docker-compose.

---

## 🚀 Orden de Ejecución

```
Fase 1 (FacturacionElectronica):
  1.1 → 1.2 → 1.3 → 1.4 → 1.5 → 1.6 → 1.7 → 1.8 → 1.9

Fase 2 (Notificaciones):
  2.1 → 2.2 → 2.3 → 2.4 → 2.5

Fase 3 (Infraestructura):
  3.1 → 3.2 → 3.3

Fase 4 (Documentación):
  4.1
```

**Tiempo estimado:** 1 sesión de trabajo (implementación completa)
