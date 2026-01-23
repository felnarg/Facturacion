# 🔧 Guía de Troubleshooting - API Gateway

## ⚠️ Problemas Comunes y Soluciones

### 1. El API Gateway no puede conectar con los microservicios

**Síntomas:**
- Error 502 Bad Gateway
- Connection refused
- Timeout errors

**Solución:**
```powershell
# 1. Verificar que todos los microservicios están ejecutándose
Get-Process | Where-Object {$_.ProcessName -eq "dotnet"}

# 2. Verificar que los puertos están en listen
netstat -ano | findstr LISTENING

# 3. Verificar puertos específicos
netstat -ano | findstr :5008  # Catalogo
netstat -ano | findstr :5013  # Clientes
netstat -ano | findstr :5011  # Compras
netstat -ano | findstr :5010  # Inventario
netstat -ano | findstr :5014  # Usuarios
netstat -ano | findstr :5012  # Ventas

# 4. Si no ve los puertos, inicie los microservicios
.\start-services.ps1
```

---

### 2. Puerto ya está en uso

**Síntomas:**
```
Address already in use
Failed to bind to address
```

**Solución:**
```powershell
# 1. Identificar qué proceso usa el puerto
$port = 5008
netstat -ano | findstr :$port

# 2. Obtener el PID (última columna)
# Ejemplo: ... LISTENING 12345
$pid = 12345

# 3. Terminar el proceso
taskkill /PID $pid /F

# 4. O terminar todos los dotnet processes
taskkill /F /IM dotnet.exe

# 5. Reintentar iniciar el servicio
```

---

### 3. Las transformaciones de rutas no funcionan

**Síntomas:**
- Rutas llegan al gateway pero fallan
- Errores 404/405

**Verificación:**
```powershell
# 1. Revisar appsettings.json del gateway
cat .\ApiGateway\ApiGateway\appsettings.json

# 2. Verificar que existen Routes y Clusters
# Routes: Define los patrones de URL
# Clusters: Define los destinos

# 3. Ejemplo de ruta correcta:
# Ruta: /api/catalog/{**remainder}
# Cluster: catalog-cluster → http://localhost:5008
# Transforms: RemovePrefix /api/catalog, AddPrefix /api
```

---

### 4. Base de datos no existe o está vacía

**Síntomas:**
- Error de conexión a BD
- Tablas no existen
- __EFMigrationsHistory no existe

**Solución:**
```powershell
# 1. Verificar cadena de conexión
# Editar: <Microservicio>/appsettings.json

# 2. Ejecutar migraciones manualmente
cd .\Catalogo\Catalogo.API
dotnet ef database update

# 3. O simplemente reiniciar el servicio (migración automática)
dotnet run

# 4. Verificar en SQL Server Management Studio
# Seleccionar BD y verificar __EFMigrationsHistory table
```

---

### 5. Error de inyección de dependencias

**Síntomas:**
```
Cannot consume scoped service from singleton
Error while validating the service descriptor
```

**Solución:**
- Ya está corregido en Clientes e Inventario
- BackgroundServices usan `IServiceScopeFactory`
- Si ocurre en otro servicio, aplicar el mismo patrón

---

### 6. Swagger no carga

**Síntomas:**
- Swagger UI en blanco
- Error 404 en /swagger

**Solución:**
```csharp
// Verificar en Program.cs:
app.UseSwagger();
app.UseSwaggerUI();

// Verificar en launchSettings.json:
"launchUrl": "swagger/index.html"
```

---

### 7. Conflicto de tipos

**Síntomas:**
```
'SaleItem' es una referencia ambigua
```

**Solución:**
- Ya está corregido en Ventas
- Usar nombre completamente cualificado:
```csharp
new Ventas.Domain.Entities.SaleItem(...)
```

---

### 8. RabbitMQ no disponible

**Síntomas:**
```
BrokerUnreachableException
None of the specified endpoints were reachable
```

**Solución:**
```powershell
# 1. Verificar que RabbitMQ está ejecutándose
# (Solo necesario si usa mensajería)

# 2. Si no lo necesita, puede desabilitar BackgroundServices
# en appsettings.json

# 3. O ejecutar RabbitMQ en Docker
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management
```

---

## 🔍 Verificación de Salud del Sistema

### Script de Verificación Completa

```powershell
# Verificar todos los puertos
function Test-Microservices {
    $services = @(
        @{ name="Catalogo"; port=5008 },
        @{ name="Clientes"; port=5013 },
        @{ name="Compras"; port=5011 },
        @{ name="Inventario"; port=5010 },
        @{ name="Usuarios"; port=5014 },
        @{ name="Ventas"; port=5012 },
        @{ name="ApiGateway"; port=5015 }
    )
    
    foreach ($service in $services) {
        try {
            $result = Invoke-WebRequest -Uri "http://localhost:$($service.port)" `
                -SkipCertificateCheck -TimeoutSec 2 -ErrorAction Stop
            Write-Host "✓ $($service.name) ($($service.port)): RESPONDE" -ForegroundColor Green
        }
        catch {
            Write-Host "✗ $($service.name) ($($service.port)): NO RESPONDE" -ForegroundColor Red
        }
    }
}

Test-Microservices
```

---

## 📊 Interpretar Logs

### Ubicación de Logs
```
C:\Users\Usuario\source\repos\Facturacion\logs\
├── Catalogo.log
├── Clientes.log
├── Compras.log
├── Inventario.log
├── Usuarios.log
├── Ventas.log
└── ApiGateway.log
```

### Ver Logs en Tiempo Real
```powershell
# PowerShell
Get-Content .\logs\Catalogo.log -Wait

# O por líneas
Get-Content .\logs\Catalogo.log -Tail 50
```

### Buscar Errores
```powershell
# Buscar errores
Select-String -Path ".\logs\*.log" -Pattern "error|Error|ERROR"

# Buscar excepciones
Select-String -Path ".\logs\*.log" -Pattern "Exception"

# Buscar migraciones
Select-String -Path ".\logs\*.log" -Pattern "migration|Migration"
```

---

## 🔄 Reinicio Completo del Sistema

```powershell
# 1. Detener todo
taskkill /F /IM dotnet.exe

# 2. Limpiar logs antiguos
Remove-Item .\logs\* -Recurse

# 3. Limpiar compilaciones
foreach ($folder in @("Catalogo", "Clientes", "Compras", "Inventario", "Usuarios", "Ventas", "ApiGateway")) {
    Remove-Item ".\$folder\*\bin" -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item ".\$folder\*\obj" -Recurse -Force -ErrorAction SilentlyContinue
}

# 4. Recompilar todo
dotnet build

# 5. Reiniciar servicios
.\start-services.ps1
```

---

## 💬 Mensajes de Error Comunes

### "Address already in use"
→ [Ver sección 2](#2-puerto-ya-está-en-uso)

### "Connection refused"
→ [Ver sección 1](#1-el-api-gateway-no-puede-conectar-con-los-microservicios)

### "Cannot consume scoped service"
→ [Ver sección 5](#5-error-de-inyección-de-dependencias)

### "No migrations were found"
→ [Ver sección 4](#4-base-de-datos-no-existe-o-está-vacía)

### "BrokerUnreachableException"
→ [Ver sección 8](#8-rabbitmq-no-disponible)

### "Es una referencia ambigua"
→ [Ver sección 7](#7-conflicto-de-tipos)

---

## 📞 Obtener Ayuda Avanzada

### Habilitar Logging Verbose

En `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Debug",
      "Microsoft.AspNetCore": "Debug"
    }
  }
}
```

### Inspeccionar Tráfico HTTP

Usar herramientas como:
- **Fiddler** - Proxy HTTP/HTTPS
- **Charles** - Monitor de tráfico
- **Postman** - Testing de APIs

---

## ✅ Checklist de Diagnóstico

```
[ ] ¿Todos los microservicios están ejecutándose?
[ ] ¿Todos los puertos están correctamente asignados?
[ ] ¿El API Gateway tiene los destinos correctos?
[ ] ¿Las bases de datos tienen migraciones aplicadas?
[ ] ¿Los logs muestran errores de conexión?
[ ] ¿Hay conflictos de puertos?
[ ] ¿La configuración de appsettings.json es correcta?
[ ] ¿RabbitMQ está disponible (si es necesario)?
[ ] ¿El firewall permite conexiones locales?
[ ] ¿Las credenciales de BD son correctas?
```

---

**Última Actualización**: 22 de Enero de 2026  
**Versión**: 1.0.0
