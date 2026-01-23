# Configuración del API Gateway

## Descripción General
El API Gateway actúa como punto de entrada único para todos los microservicios. Utiliza YARP (Yet Another Reverse Proxy) para enrutar las solicitudes a los microservicios correspondientes.

## Configuración de Rutas

### Rutas Disponibles

| Microservicio | Ruta Gateway | Endpoint | Puerto |
|---------------|--------------|----------|--------|
| **Catalogo** | `/api/catalog/{**remainder}` | `http://localhost:5008` | 5008 |
| **Clientes** | `/api/customers/{**remainder}` | `http://localhost:5013` | 5013 |
| **Compras** | `/api/purchases/{**remainder}` | `http://localhost:5011` | 5011 |
| **Inventario** | `/api/inventory/{**remainder}` | `http://localhost:5010` | 5010 |
| **Usuarios** | `/api/auth/{**remainder}` | `http://localhost:5014` | 5014 |
| **Ventas** | `/api/sales/{**remainder}` | `http://localhost:5012` | 5012 |

## Transformaciones de Rutas

Cada ruta incluye transformaciones para:
1. **Remover prefijo de la ruta del gateway**: Quita el prefijo específico del gateway (ej: `/api/catalog`)
2. **Agregar prefijo de la API**: Agrega el prefijo `/api` para que los endpoints locales reciban las solicitudes correctamente

Ejemplo:
- Solicitud al Gateway: `GET /api/catalog/products`
- Solicitud transformada al microservicio: `GET http://localhost:5008/api/products`

## Configuración de Desarrollo

- **Entorno**: `appsettings.Development.json` sobrescribe la configuración base
- **Hosts**: Todos los microservicios están configurados en `localhost` para desarrollo local
- **Puertos**: Cada microservicio tiene su puerto único asignado

## Cambios Recientes

### Actualización de Destinos (22-01-2026)

Se actualizaron los destinos de los clusters para usar endpoints locales en lugar de nombres de host de Docker:

**Antes (Docker):**
```json
"catalog-cluster": {
  "Address": "http://catalogo-api"
}
```

**Después (Local):**
```json
"catalog-cluster": {
  "Address": "http://localhost:5008"
}
```

### Microservicios Verificados

Todos los microservicios fueron compilados y ejecutados para validar las migraciones de base de datos:

✅ Catalogo - Puerto 5008
✅ Clientes - Puerto 5013  
✅ Compras - Puerto 5011
✅ Inventario - Puerto 5010
✅ Usuarios - Puerto 5014
✅ Ventas - Puerto 5012

## Próximos Pasos

1. Ejecutar el script de inicio: `.\start-services.ps1`
2. O ejecutar cada microservicio y el API Gateway manualmente en sus puertos respectivos
3. Acceder al API Gateway en `http://localhost:5015`
4. Verificar que el enrutamiento funciona correctamente

## URLs de Acceso

**Gateway:**
- http://localhost:5015

**Microservicios (directos):**
- Catalogo: http://localhost:5008/swagger
- Clientes: http://localhost:5013/swagger
- Compras: http://localhost:5011/swagger
- Inventario: http://localhost:5010/swagger
- Usuarios: http://localhost:5014/swagger
- Ventas: http://localhost:5012/swagger

**Rutas a través del Gateway:**
- Catalogo: http://localhost:5015/api/catalog/...
- Clientes: http://localhost:5015/api/customers/...
- Compras: http://localhost:5015/api/purchases/...
- Inventario: http://localhost:5015/api/inventory/...
- Usuarios: http://localhost:5015/api/auth/...
- Ventas: http://localhost:5015/api/sales/...

## Notas

- El API Gateway debe ejecutarse **después** de que todos los microservicios estén en línea
- La configuración de `appsettings.Development.json` prevalece sobre `appsettings.json` cuando se ejecuta en ambiente Development
- Los puertos están basados en la configuración de `launchSettings.json` de cada microservicio
