# Mapeo de Puertos y Rutas - API Gateway

## 📊 Tabla de Referencia Rápida

```
┌─────────────────┬──────────┬────────────────────────┬──────────────────────────┐
│ Microservicio   │ Puerto   │ Ruta Gateway           │ Endpoint Local           │
├─────────────────┼──────────┼────────────────────────┼──────────────────────────┤
│ Catalogo        │ 5008     │ /api/catalog/**        │ http://localhost:5008    │
│ Clientes        │ 5013     │ /api/customers/**      │ http://localhost:5013    │
│ Compras         │ 5011     │ /api/purchases/**      │ http://localhost:5011    │
│ Inventario      │ 5010     │ /api/inventory/**      │ http://localhost:5010    │
│ Usuarios (Auth) │ 5014     │ /api/auth/**           │ http://localhost:5014    │
│ Ventas          │ 5012     │ /api/sales/**          │ http://localhost:5012    │
│ API Gateway     │ 5015     │ -                      │ http://localhost:5015    │
└─────────────────┴──────────┴────────────────────────┴──────────────────────────┘
```

## 🔄 Flujo de Enrutamiento

### Ejemplo 1: Obtener productos del catálogo
```
Cliente
   ↓
GET /api/catalog/products → API Gateway (5015)
   ↓
[Transformación]
- RemovePrefix: /api/catalog → /products
- AddPrefix: /api → /api/products
   ↓
GET /api/products → Catalogo (5008)
   ↓
Respuesta del servidor
```

### Ejemplo 2: Crear usuario
```
Cliente
   ↓
POST /api/auth/users → API Gateway (5015)
   ↓
[Transformación]
- RemovePrefix: /api/auth → /users
- AddPrefix: /api → /api/users
   ↓
POST /api/users → Usuarios (5014)
   ↓
Respuesta del servidor
```

## 🚀 Comandos para Iniciar Servicios

### Opción 1: Script Automático
```powershell
cd C:\Users\Usuario\source\repos\Facturacion
.\start-services.ps1
```

### Opción 2: Manual - Catalogo
```powershell
cd C:\Users\Usuario\source\repos\Facturacion\Catalogo\Catalogo.API
dotnet run
```

### Opción 3: Manual - Todo
```powershell
# Terminal 1 - Catalogo
cd C:\Users\Usuario\source\repos\Facturacion\Catalogo\Catalogo.API && dotnet run

# Terminal 2 - Clientes
cd C:\Users\Usuario\source\repos\Facturacion\Clientes\Clientes.API && dotnet run

# Terminal 3 - Compras
cd C:\Users\Usuario\source\repos\Facturacion\Compras\Compras.API && dotnet run

# Terminal 4 - Inventario
cd C:\Users\Usuario\source\repos\Facturacion\Inventario\Inventario.API && dotnet run

# Terminal 5 - Usuarios
cd C:\Users\Usuario\source\repos\Facturacion\Usuarios\Usuarios.API && dotnet run

# Terminal 6 - Ventas
cd C:\Users\Usuario\source\repos\Facturacion\Ventas\Ventas.API && dotnet run

# Terminal 7 - API Gateway
cd C:\Users\Usuario\source\repos\Facturacion\ApiGateway\ApiGateway && dotnet run
```

## 🧪 Pruebas de Conectividad

### Verificar que todos los servicios están activos
```bash
# Catalogo
curl http://localhost:5008/swagger

# Clientes
curl http://localhost:5013/swagger

# Compras
curl http://localhost:5011/swagger

# Inventario
curl http://localhost:5010/swagger

# Usuarios
curl http://localhost:5014/swagger

# Ventas
curl http://localhost:5012/swagger

# API Gateway
curl http://localhost:5015
```

### Pruebas a través del Gateway (si está disponible)
```bash
# Ejemplo: GET /api/catalog/products
curl http://localhost:5015/api/catalog/products

# Ejemplo: POST /api/auth/login
curl -X POST http://localhost:5015/api/auth/login

# Ejemplo: GET /api/inventory/stock
curl http://localhost:5015/api/inventory/stock
```

## 🔧 Configuración en Archivos

### appsettings.json (Base)
- Contiene configuración compartida y valores por defecto
- Se sobrescribe por `appsettings.Development.json` en ambiente Development

### appsettings.Development.json (Desarrollo)
- Configuración específica para ambiente de desarrollo
- Prevale sobre appsettings.json cuando ASPNETCORE_ENVIRONMENT=Development
- Ya tiene todos los puertos locales configurados

## ⚠️ Troubleshooting

### El gateway no puede conectar con un microservicio
1. Verificar que el microservicio está ejecutándose en su puerto
2. Verificar que el puerto en appsettings.json es correcto
3. Revisar logs del gateway para errores de conexión

### Puerto ya en uso
```powershell
# Encontrar proceso usando puerto 5008
netstat -ano | findstr :5008

# Matar proceso por PID
taskkill /PID 12345 /F
```

### Transformaciones de rutas no funcionan
1. Verificar que las rutas en `Routes` y `Clusters` coinciden
2. Revisar la configuración de `Transforms`
3. Consultar logs del reverse proxy para detalles

## 📝 Notas Importantes

- El API Gateway DEBE iniciarse **después** que todos los microservicios
- Los puertos están configurados en `launchSettings.json` de cada servicio
- En Docker/Producción, cambiar direcciones a nombres de servicios (ej: `http://catalogo:80`)
- Cada microservicio tiene su propia base de datos
- RabbitMQ debe estar disponible para mensajería entre servicios
