# Sistema de Facturación - Microservicios

## 🎯 Visión General

Sistema de facturación escalable basado en arquitectura de microservicios. Cada componente es independiente con su propia base de datos y puede desarrollarse, desplegarse y escalarse de forma autónoma.

## 🏗️ Arquitectura

```
┌─────────────────────────────────────────────────────────────────┐
│                    CLIENTE / NAVEGADOR                          │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│          API GATEWAY (YARP - Reverse Proxy)                     │
│                   Puerto: 5015                                  │
└────────────────────────────┬────────────────────────────────────┘
          │                  │                  │                  │
          ▼                  ▼                  ▼                  ▼
    ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐
    │ Catalogo    │  │ Clientes    │  │ Compras     │  │ Inventario  │
    │ (5008)      │  │ (5013)      │  │ (5011)      │  │ (5010)      │
    └──────┬──────┘  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘
           │                │                │                │
           ▼                ▼                ▼                ▼
    ┌──────────┐     ┌──────────┐     ┌──────────┐     ┌──────────┐
    │ Catalogo │     │ Clientes │     │ Compras  │     │Inventario│
    │   DB     │     │   DB     │     │   DB     │     │   DB     │
    └──────────┘     └──────────┘     └──────────┘     └──────────┘
          │                │                │                │
          └────────────────┼────────────────┼────────────────┘
                           │
                           ▼
                  ┌──────────────────┐
                  │   RabbitMQ       │
                  │  Mensajería      │
                  └──────────────────┘

    ┌─────────────┐                              ┌─────────────┐
    │ Usuarios    │                              │ Ventas      │
    │ (5014)      │                              │ (5012)      │
    └──────┬──────┘                              └──────┬──────┘
           │                                           │
           ▼                                           ▼
    ┌──────────┐                                ┌──────────┐
    │ Usuarios │                                │ Ventas   │
    │   DB     │                                │   DB     │
    └──────────┘                                └──────────┘
```

## 📦 Microservicios

| Servicio | Puerto | Ruta | Base de Datos | Descripción |
|----------|--------|------|---------------|-------------|
| **Catalogo** | 5008 | /api/catalog | Catalogo | Gestión de productos y categorías |
| **Clientes** | 5013 | /api/customers | Clientes | Gestión de clientes y su historial de compras |
| **Compras** | 5011 | /api/purchases | Compras | Gestión de órdenes de compra |
| **Inventario** | 5010 | /api/inventory | Inventario | Control de stock y disponibilidad |
| **Usuarios** | 5014 | /api/auth | Usuarios | Autenticación y autorización |
| **Ventas** | 5012 | /api/sales | Ventas | Gestión de ventas y facturas |

## 🚀 Inicio Rápido

### Requisitos Previos
- .NET 10.0 SDK o superior
- SQL Server 2019+ (o compatible)
- RabbitMQ (opcional, solo si se necesita mensajería)

### Paso 1: Clonar/Abrir el Proyecto
```bash
cd C:\Users\Usuario\source\repos\Facturacion
```

### Paso 2: Iniciar Todos los Servicios (Recomendado)
```powershell
.\start-services.ps1
```

### Paso 3: O Iniciar Manualmente
En terminal PowerShell separadas:

```powershell
# Terminal 1 - Catalogo
cd .\Catalogo\Catalogo.API && dotnet run

# Terminal 2 - Clientes
cd .\Clientes\Clientes.API && dotnet run

# Terminal 3 - Compras
cd .\Compras\Compras.API && dotnet run

# Terminal 4 - Inventario
cd .\Inventario\Inventario.API && dotnet run

# Terminal 5 - Usuarios
cd .\Usuarios\Usuarios.API && dotnet run

# Terminal 6 - Ventas
cd .\Ventas\Ventas.API && dotnet run

# Terminal 7 - API Gateway
cd .\ApiGateway\ApiGateway && dotnet run
```

## 🌐 Acceso a los Servicios

### API Gateway (Recomendado)
- **URL**: http://localhost:5015
- Proporciona acceso unificado a todos los microservicios

### Acceso Directo a Swagger
- Catalogo: http://localhost:5008/swagger
- Clientes: http://localhost:5013/swagger
- Compras: http://localhost:5011/swagger
- Inventario: http://localhost:5010/swagger
- Usuarios: http://localhost:5014/swagger
- Ventas: http://localhost:5012/swagger

## 📚 Documentación

### Archivos Disponibles

- **API-GATEWAY-CONFIG.md** - Configuración detallada del API Gateway
- **PUERTO-ROUTES-MAPPING.md** - Mapeo de puertos y rutas
- **Interaccion-Modulos.md** - Descripción de cómo interactúan los módulos
- **RoadMap.md** - Roadmap del proyecto

## 🔧 Estructura del Código

Cada microservicio sigue la arquitectura limpia:

```
MicroServicio/
├── MicroServicio.API/           # Capa de Presentación (Controllers)
├── MicroServicio.Application/   # Lógica de Negocio (Services, DTOs)
├── MicroServicio.Domain/        # Entidades y Repositorios
└── MicroServicio.Infrastructure/# Acceso a Datos, EF Core, Messaging
```

## 🗄️ Base de Datos

### Configuración
- **Tipo**: SQL Server
- **Migraciones**: Automáticas al iniciar cada servicio
- **Cadena de Conexión**: Configurada en `appsettings.json`

### Migraciones Automáticas
Cada microservicio ejecuta automáticamente sus migraciones al iniciarse:

```csharp
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
    dbContext.Database.Migrate();
}
```

## 🔄 Comunicación Entre Servicios

### Sincrónica (HTTP)
- Llamadas directas entre microservicios
- A través del API Gateway

### Asincrónica (Mensajería)
- **RabbitMQ** para eventos de dominio
- Ejemplo: Evento `SaleCompleted` notifica a Clientes e Inventario

## ⚙️ Configuración

### Variables de Entorno
```
ASPNETCORE_ENVIRONMENT=Development  (o Production)
```

### Archivos de Configuración
- `appsettings.json` - Configuración base
- `appsettings.Development.json` - Overrides para Development

## 🐛 Troubleshooting

### Puerto ya en uso
```powershell
# Identificar proceso
netstat -ano | findstr :5008

# Terminar proceso
taskkill /PID <PID> /F
```

### API Gateway no conecta con microservicio
1. Verificar que el microservicio está ejecutándose
2. Verificar puerto en `launchSettings.json`
3. Revisar `appsettings.json` del gateway

### Migraciones no aplican
- Verificar conexión a SQL Server
- Revisar carpeta Migrations en cada proyecto
- Ejecutar manualmente: `dotnet ef database update`

## 📋 Cambios Recientes (22-01-2026)

### ✅ Migraciones Ejecutadas
- Todas las bases de datos creadas y actualizadas
- Tablas `__EFMigrationsHistory` inicializadas

### ✅ Correcciones de Dependencias
- Clientes: BackgroundService corregido con `IServiceScopeFactory`
- Inventario: BackgroundService corregido con `IServiceScopeFactory`

### ✅ Configuración del Gateway
- Endpoints actualizados de Docker a localhost
- Todos los puertos verificados y configurados
- Rutas validadas

## 🔐 Seguridad

- Autenticación JWT en el microservicio de Usuarios
- Todas las solicitudes a través del Gateway
- HTTPS en producción (configurado en Dockerfile)

## 📞 Soporte

Para más información, consulte:
- `Interaccion-Modulos.md` - Interacción entre módulos
- `RoadMap.md` - Planes futuros

---

**Versión**: 1.0.0  
**Última Actualización**: 22 de Enero de 2026  
**Estado**: ✅ Operacional
