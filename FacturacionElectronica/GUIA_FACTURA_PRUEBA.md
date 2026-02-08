# 📄 Guía para Generar Factura Electrónica de Prueba

Esta guía te ayudará a generar una factura electrónica de prueba usando el microservicio de Facturación Electrónica.

## 📋 Datos de Prueba Disponibles (Seed)

El sistema incluye datos de prueba que se cargan automáticamente:

### Emisor
- **NIT**: `9001234567`
- **Razón Social**: `EMPRESA DE PRUEBA SAS`
- **Numeración Factura**: Prefijo `SETP`, rango 1-100000

### Clientes Disponibles
1. **Cliente Empresarial**
   - **Identificación**: `8001234567`
   - **Razón Social**: `CLIENTE EMPRESARIAL SAS`

2. **Consumidor Final**
   - **Identificación**: `222222222222`
   - **Razón Social**: `CONSUMIDOR FINAL`

## 🚀 Generar Factura Electrónica

### Endpoint
```
POST http://localhost:8087/api/DocumentoElectronico
```

### Ejemplo 1: Factura Simple con IVA

```json
{
  "tipoDocumento": 1,
  "fechaEmision": "2026-02-08T10:00:00Z",
  "fechaVencimiento": "2026-02-15T10:00:00Z",
  "observaciones": "Factura de prueba",
  "clienteId": "8001234567",
  "items": [
    {
      "orden": 1,
      "codigo": "PROD001",
      "descripcion": "Producto de Prueba 1",
      "cantidad": 2,
      "unidadMedida": 1,
      "valorUnitario": {
        "valor": 50000,
        "moneda": "COP"
      },
      "observaciones": "Item de prueba"
    },
    {
      "orden": 2,
      "codigo": "PROD002",
      "descripcion": "Producto de Prueba 2",
      "cantidad": 1,
      "unidadMedida": 1,
      "valorUnitario": {
        "valor": 75000,
        "moneda": "COP"
      }
    }
  ],
  "impuestos": [
    {
      "tipoImpuesto": 1,
      "porcentaje": 19,
      "baseGravable": {
        "valor": 175000,
        "moneda": "COP"
      },
      "codigoTributo": "01",
      "nombreTributo": "IVA"
    }
  ]
}
```

### Ejemplo 2: Factura con Descuentos

```json
{
  "tipoDocumento": 1,
  "fechaEmision": "2026-02-08T10:00:00Z",
  "fechaVencimiento": "2026-02-15T10:00:00Z",
  "observaciones": "Factura con descuento",
  "clienteId": "222222222222",
  "items": [
    {
      "orden": 1,
      "codigo": "PROD003",
      "descripcion": "Producto con Descuento",
      "cantidad": 3,
      "unidadMedida": 1,
      "valorUnitario": {
        "valor": 100000,
        "moneda": "COP"
      },
      "descuento": {
        "valor": 50000,
        "moneda": "COP"
      }
    }
  ],
  "impuestos": [
    {
      "tipoImpuesto": 1,
      "porcentaje": 19,
      "baseGravable": {
        "valor": 250000,
        "moneda": "COP"
      },
      "codigoTributo": "01",
      "nombreTributo": "IVA"
    }
  ]
}
```

### Ejemplo 3: Factura para Consumidor Final

```json
{
  "tipoDocumento": 1,
  "fechaEmision": "2026-02-08T10:00:00Z",
  "fechaVencimiento": "2026-02-15T10:00:00Z",
  "observaciones": "Factura consumidor final",
  "clienteId": "222222222222",
  "items": [
    {
      "orden": 1,
      "codigo": "SERV001",
      "descripcion": "Servicio de Consultoría",
      "cantidad": 1,
      "unidadMedida": 1,
      "valorUnitario": {
        "valor": 500000,
        "moneda": "COP"
      }
    }
  ],
  "impuestos": [
    {
      "tipoImpuesto": 1,
      "porcentaje": 19,
      "baseGravable": {
        "valor": 500000,
        "moneda": "COP"
      },
      "codigoTributo": "01",
      "nombreTributo": "IVA"
    }
  ]
}
```

## 📝 Tipos de Documento

- `1` = FacturaElectronica (Normal)
- `2` = FacturaElectronicaExportacion
- `3` = FacturaElectronicaContingencia
- `4` = NotaCredito
- `5` = NotaDebito

## 📊 Unidades de Medida

- `1` = Unidad
- `2` = Kilogramo
- `3` = Litro
- `4` = Metro
- `5` = MetroCuadrado
- `6` = MetroCubico
- `7` = Paquete
- `8` = Caja

## 💰 Tipos de Impuesto

- `1` = IVA
- `2` = ICA
- `3` = INPP
- `4` = IBUA
- `5` = ICIU
- `6` = ICL
- `7` = RetencionIVA
- `8` = RetencionFuente

## 🔍 Consultar Facturas Generadas

### Obtener todas las facturas
```
GET http://localhost:8087/api/DocumentoElectronico
```

### Obtener factura por ID
```
GET http://localhost:8087/api/DocumentoElectronico/{id}
```

### Obtener facturas por emisor
```
GET http://localhost:8087/api/DocumentoElectronico/emisor/9001234567
```

### Obtener facturas por cliente
```
GET http://localhost:8087/api/DocumentoElectronico/cliente/8001234567
```

## 🧪 Probar con cURL

### Ejemplo básico con cURL

```bash
curl -X POST http://localhost:8087/api/DocumentoElectronico \
  -H "Content-Type: application/json" \
  -d '{
    "tipoDocumento": 1,
    "fechaEmision": "2026-02-08T10:00:00Z",
    "fechaVencimiento": "2026-02-15T10:00:00Z",
    "observaciones": "Factura de prueba",
    "clienteId": "8001234567",
    "items": [
      {
        "orden": 1,
        "codigo": "PROD001",
        "descripcion": "Producto de Prueba",
        "cantidad": 2,
        "unidadMedida": 1,
        "valorUnitario": {
          "valor": 50000,
          "moneda": "COP"
        }
      }
    ],
    "impuestos": [
      {
        "tipoImpuesto": 1,
        "porcentaje": 19,
        "baseGravable": {
          "valor": 100000,
          "moneda": "COP"
        },
        "codigoTributo": "01",
        "nombreTributo": "IVA"
      }
    ]
  }'
```

## 🧪 Probar con PowerShell

```powershell
$body = @{
    tipoDocumento = 1
    fechaEmision = "2026-02-08T10:00:00Z"
    fechaVencimiento = "2026-02-15T10:00:00Z"
    observaciones = "Factura de prueba"
    clienteId = "8001234567"
    items = @(
        @{
            orden = 1
            codigo = "PROD001"
            descripcion = "Producto de Prueba"
            cantidad = 2
            unidadMedida = 1
            valorUnitario = @{
                valor = 50000
                moneda = "COP"
            }
        }
    )
    impuestos = @(
        @{
            tipoImpuesto = 1
            porcentaje = 19
            baseGravable = @{
                valor = 100000
                moneda = "COP"
            }
            codigoTributo = "01"
            nombreTributo = "IVA"
        }
    )
} | ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri "http://localhost:8087/api/DocumentoElectronico" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body
```

## 📊 Respuesta Esperada

Al crear una factura exitosamente, recibirás una respuesta similar a:

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "numeroDocumento": "SETP0000000000000000001",
  "tipoDocumento": 1,
  "estado": 1,
  "fechaEmision": "2026-02-08T10:00:00Z",
  "fechaVencimiento": "2026-02-15T10:00:00Z",
  "observaciones": "Factura de prueba",
  "emisorId": "9001234567",
  "clienteId": "8001234567",
  "subtotal": {
    "valor": 100000,
    "moneda": "COP"
  },
  "totalDescuentos": {
    "valor": 0,
    "moneda": "COP"
  },
  "totalImpuestos": {
    "valor": 19000,
    "moneda": "COP"
  },
  "total": {
    "valor": 119000,
    "moneda": "COP"
  },
  "totalPagado": {
    "valor": 0,
    "moneda": "COP"
  },
  "saldoPendiente": {
    "valor": 119000,
    "moneda": "COP"
  },
  "items": [...],
  "impuestos": [...],
  "pagos": [],
  "eventos": [...]
}
```

## ⚠️ Notas Importantes

1. **Estado del Documento**: Al crear la factura, el estado será `1` (Pendiente)
2. **Numeración Automática**: El sistema genera automáticamente el número de documento usando la numeración autorizada
3. **Cálculos Automáticos**: Los totales se calculan automáticamente al agregar items e impuestos
4. **Validaciones**: El sistema valida que:
   - El cliente exista
   - El emisor tenga numeración activa
   - Las fechas sean válidas
   - Los valores monetarios sean positivos

## 🔄 Próximos Pasos

Una vez creada la factura, los siguientes pasos serían:
1. **Generar XML** (endpoint futuro: `POST /api/DocumentoElectronico/{id}/generar`)
2. **Firmar Documento** (endpoint futuro: `POST /api/DocumentoElectronico/{id}/firmar`)
3. **Transmitir a DIAN** (endpoint futuro: `POST /api/DocumentoElectronico/{id}/transmitir`)

## 📚 Referencias

- Skills DIAN: Ver `Skills/00-Portada-Contenido.md.txt`
- Estructura Invoice: `Skills/03-Estructura-Invoice.md`
- Reglas Validación: `Skills/04-Reglas-Validacion.md`