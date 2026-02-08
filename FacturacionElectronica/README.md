# Microservicio de Facturación Electrónica DIAN

Microservicio para la gestión de facturación electrónica según los estándares de la DIAN Colombiana.

## 🏗️ Arquitectura

El microservicio sigue una arquitectura Clean Architecture con los siguientes layers:

### 1. **Domain Layer** (`FacturacionElectronica.Domain`)
- **Entidades**: Emisor, Cliente, DocumentoElectronico, NumeracionDocumento, ItemDocumento, ImpuestoDocumento, PagoDocumento, EventoDocumento
- **Value Objects**: Direccion, InformacionContacto, ValorMonetario
- **Enums**: TipoDocumento, TipoPersona, TipoResponsabilidadFiscal, TipoImpuesto, EstadoDocumento, UnidadMedida
- **Interfaces**: Repositorios y Unit of Work
- **Eventos de Dominio**: DocumentoGeneradoEvent, DocumentoFirmadoEvent, etc.

### 2. **Application Layer** (`FacturacionElectronica.Application`)
- **Commands**: Casos de uso para operaciones de escritura
- **Queries**: Casos de uso para operaciones de lectura
- **DTOs**: Objetos de transferencia de datos
- **Validators**: Validaciones con FluentValidation
- **Mappings**: Configuración de AutoMapper
- **Services**: Servicios de aplicación

### 3. **Infrastructure Layer** (`FacturacionElectronica.Infrastructure`)
- **Data**: DbContext, Unit of Work, Configuraciones de Entity Framework
- **Repositories**: Implementaciones de repositorios
- **Services**: Servicios de infraestructura
- **EventBus**: Integración con RabbitMQ
- **Migrations**: Migraciones de base de datos

### 4. **API Layer** (`FacturacionElectronica.API`)
- **Controllers**: Endpoints REST
- **Middlewares**: Pipeline de la aplicación
- **Configuración**: Dependency Injection, Swagger, etc.

## 📋 Características Principales

### ✅ Cumplimiento DIAN
- Generación de XML según estándar UBL 2.1
- Cálculo de CUFE/CUDE según resolución DIAN
- Generación de códigos QR
- Firma digital XAdES-EPES
- Comunicación con Web Services DIAN

### ✅ Gestión de Documentos
- Facturas electrónicas (tipo 01, 02, 03)
- Notas crédito y débito
- Documentos equivalentes
- Estados del documento: Pendiente → Generado → Firmado → Transmitido → Aceptado/Rechazado

### ✅ Validaciones
- Validación de reglas DIAN
- Validación de numeración autorizada
- Validación de cálculos aritméticos
- Validación de estructura XML

### ✅ Comunicación
- API REST para integración
- Eventos con RabbitMQ para comunicación asíncrona
- Web Services SOAP para comunicación con DIAN

## 🚀 Despliegue

### Requisitos
- Docker y Docker Compose
- SQL Server 2022
- RabbitMQ 3.8+

### Configuración Docker
El microservicio está configurado para ejecutarse en el stack Docker existente:

```yaml
facturacion-electronica-api:
  image: facturacion-electronica-api-img
  build:
    context: .
    dockerfile: FacturacionElectronica/FacturacionElectronica.API/Dockerfile
  environment:
    - ConnectionStrings__DefaultConnection=Data Source=sqlserver,1433;Initial Catalog=FacturacionElectronica;...
    - RabbitMQ__HostName=rabbitmq
    - RabbitMQ__UserName=admin
    - RabbitMQ__Password=p@ssword123
  ports:
    - "8087:80"
```

### Variables de Entorno
- `ConnectionStrings__DefaultConnection`: Cadena de conexión a SQL Server
- `RabbitMQ__*`: Configuración de RabbitMQ
- `DIAN__*`: Configuración de servicios DIAN
- `ASPNETCORE_ENVIRONMENT`: Entorno de ejecución

## 📊 Base de Datos

### Esquema Principal
```
FacturacionElectronica/
├── Emisores/              # Información de emisores autorizados
├── Clientes/              # Información de clientes/receptores
├── DocumentosElectronicos/# Documentos electrónicos
├── Numeraciones/          # Numeraciones autorizadas por la DIAN
├── ItemsDocumentos/       # Items de los documentos
├── ImpuestosDocumentos/   # Impuestos aplicados
├── PagosDocumentos/       # Pagos recibidos
└── EventosDocumentos/     # Historial de eventos
```

### Migraciones
```bash
# Desde el directorio del proyecto Infrastructure
dotnet ef migrations add InitialCreate --context ApplicationDbContext
dotnet ef database update --context ApplicationDbContext
```

## 🔌 API Endpoints

### Emisores
- `GET /api/emisor` - Listar emisores
- `GET /api/emisor/{nit}` - Obtener emisor por NIT
- `POST /api/emisor` - Crear emisor
- `PUT /api/emisor/{nit}` - Actualizar emisor
- `POST /api/emisor/{nit}/numeracion` - Agregar numeración

### Clientes
- `GET /api/cliente` - Listar clientes
- `GET /api/cliente/{identificacion}` - Obtener cliente
- `POST /api/cliente` - Crear cliente
- `PUT /api/cliente/{identificacion}` - Actualizar cliente

### Documentos Electrónicos
- `GET /api/documento` - Listar documentos
- `GET /api/documento/{id}` - Obtener documento
- `POST /api/documento` - Crear documento
- `POST /api/documento/{id}/generar` - Generar XML
- `POST /api/documento/{id}/firmar` - Firmar documento
- `POST /api/documento/{id}/transmitir` - Transmitir a DIAN
- `GET /api/documento/emisor/{emisorId}` - Documentos por emisor
- `GET /api/documento/cliente/{clienteId}` - Documentos por cliente

## 🔄 Eventos RabbitMQ

### Eventos Publicados
- `documento.generado` - Cuando se genera un documento
- `documento.firmado` - Cuando se firma un documento
- `documento.transmitido` - Cuando se transmite a DIAN
- `documento.aceptado` - Cuando DIAN acepta el documento
- `documento.rechazado` - Cuando DIAN rechaza el documento

### Configuración
- **Exchange**: `facturacion.events`
- **Queue**: `facturacion.electronica.events`
- **Routing Key**: `documento.*`

## 🧪 Testing

### Pruebas Unitarias
```bash
# Ejecutar pruebas
dotnet test
```

### Pruebas de Integración
- Validación de XML contra XSD DIAN
- Pruebas de comunicación con servicios DIAN (ambiente de pruebas)
- Pruebas de firma digital

## 📈 Monitoreo

### Health Checks
- `GET /health` - Estado del servicio
- `GET /health/ready` - Ready check
- `GET /health/live` - Live check

### Métricas
- Documentos procesados por hora
- Tasa de éxito/rechazo DIAN
- Tiempos de respuesta
- Uso de recursos

## 🔒 Seguridad

### Autenticación
- Integración con microservicio de Usuarios
- Tokens JWT
- Roles y permisos

### Certificados Digitales
- Almacenamiento seguro de certificados
- Firma digital con ECDSA
- Rotación de certificados

## 📚 Documentación Adicional

### Skills DIAN
El microservicio implementa las skills documentadas en:
- `Skills/00-Portada-Contenido.md.txt` - Mapa conceptual completo
- `Skills/03-Estructura-Invoice.md` - Estructura de factura
- `Skills/04-Reglas-Validacion.md` - Reglas de validación
- `Skills/07-Transmision-WebServices.md` - Web Services DIAN
- `Skills/08-Suplemento-A-Firma-Digital.md` - Firma digital
- `Skills/09-Suplemento-B-CUFE-CUDE.md` - CUFE/CUDE/QR

### Patrones Implementados
- **Repository Pattern**: Abstracción de acceso a datos
- **Unit of Work**: Transacciones coordinadas
- **CQRS**: Separación de comandos y consultas
- **Domain Events**: Eventos de dominio para desacoplamiento
- **Value Objects**: Objetos inmutables para conceptos del dominio

## 🛠️ Desarrollo

### Estructura de Commits
- `feat:` Nueva característica
- `fix:` Corrección de bug
- `docs:` Documentación
- `style:` Formato (sin cambios funcionales)
- `refactor:` Refactorización de código
- `test:` Pruebas
- `chore:` Tareas de mantenimiento

### Code Style
- C# 12 features
- Async/await pattern
- Nullable reference types
- Fluent validation
- AutoMapper para DTOs

## 🤝 Contribución

1. Fork el repositorio
2. Crear feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit cambios (`git commit -m 'feat: Add AmazingFeature'`)
4. Push al branch (`git push origin feature/AmazingFeature`)
5. Abrir Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT.

## 🆘 Soporte

Para soporte técnico:
1. Revisar documentación en `Skills/`
2. Consultar logs de aplicación
3. Verificar estado de servicios DIAN
4. Contactar al equipo de desarrollo

---
**Última actualización**: Febrero 2026  
**Versión**: 1.0.0  
**Estado**: Desarrollo Activo