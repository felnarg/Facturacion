# Microservicios

### Hoja de Ruta Cronológica (desarrollo por fases)

Para construir un sistema **ensamblable**, es mejor avanzar por dependencias. Esta ruta está organizada por fases.

---

### Fase 1: Infraestructura (Semana 1)

Antes de programar lógica de negocio, prepara el **esqueleto** donde vivirán los módulos.

1. **Shared Kernel**
    - Crea una librería compartida (por ejemplo, NuGet en .NET) con eventos comunes.
    - Ejemplos: `ProductCreated`, `StockDecreased`.
2. **Message Broker**
    - Levanta un contenedor con **RabbitMQ** o **Redis**.
3. **API Gateway**
    - Configura **YARP** (Microsoft) u **Ocelot** para redirigir las peticiones.

---

### Fase 2: Cimientos (módulos base)

1. **User Service (IAM)**
    - Define autenticación y autorización.
    - Sin seguridad es difícil probar el resto de módulos.
2. **Catalog Service**
    - Implementa el CRUD de productos.
    - **Evento:** al crear un producto, publica `ProductCreated`.

---

### Fase 3: Abastecimiento (compras e inventario)

1. **Inventory Service**
    - Escucha `ProductCreated` del Catálogo.
    - Crea el registro de stock inicial en **0**.
2. **Purchase Service (Compras)**
    - Gestiona proveedores y recepciones.
    - **Evento:** cuando marcas “Compra recibida”, publica `StockReceived`.
    - **Reacción:** Inventario escucha `StockReceived` y aumenta existencias.

---

### Fase 4: Ventas (motor de dinero)

1. **Sales Service**
    - POS (Punto de Venta).
    - Consulta Catálogo para precios e Inventario para disponibilidad.
    - **Evento:** al finalizar la venta, publica `SaleCompleted`.
    - **Reacción:**
        - Inventario escucha `SaleCompleted` y descuenta productos.
        - Finance (si existe) escucha y genera la factura.

---

### Resumen del flujo de datos

| **Paso** | **Acción del usuario** | **Módulo que actúa** | **Evento generado** | **Quién reacciona** |
| --- | --- | --- | --- | --- |
| 1 | Crea “Leche” | Catálogo | `ProductCreated` | Inventario (crea stock en 0) |
| 2 | Compra 100 bolsas | Compras | `StockReceived` | Inventario (suma 100) |
| 3 | Cliente compra 1 | Ventas | `SaleCompleted` | Inventario (resta 1) |

---

### ¿Por dónde empezar hoy mismo?

**Recomendación:** empieza por el **Catalog Service** y el **API Gateway**. Te darán progreso visible rápido porque podrás ver productos en el frontend casi de inmediato.

---

Para que tu sistema de supermercado sea profesional y "ensamblable", esta primera fase es crítica. Vamos a construir el sistema nervioso (Shared Kernel), el cartero (Message Broker) y el portero (Gateway).

Aquí tienes la guía técnica paso a paso:

---

### 1. El Shared Kernel (Tu librería NuGet interna)

Este proyecto no es una API, es una **Class Library (.NET)**. Su objetivo es evitar que tengas que escribir la misma clase `ProductCreated` en todos los microservicios.

**Pasos:**

1. Crea una solución en Visual Studio o VS Code: `Supermercado.Modular`.
2. Crea un proyecto de tipo **Class Library** llamado `Supermercado.Shared`.
3. Dentro, crea una carpeta llamada `Events`.
4. Define tus registros (records) de eventos. Usa `record` porque son inmutables:

C#

`namespace Supermercado.Shared.Events;

// Evento base para que todos tengan un ID y fecha
public record IntegrationEvent(Guid Id, DateTime OccurredOn);

public record ProductCreated(
    Guid ProductId, 
    string Name, 
    string Sku, 
    decimal Price
) : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

public record StockDecreased(
    Guid ProductId, 
    int Quantity
) : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);`

> Tip para comercializar: Cuando desarrolles los módulos reales, solo tendrás que añadir la referencia a este proyecto (o un paquete NuGet privado) para que todos hablen el mismo idioma.
> 

---

### 2. Message Broker (RabbitMQ con Docker)

Para que los eventos viajen, necesitamos a RabbitMQ. Lo configuraremos dentro de un archivo `docker-compose.yml` en la raíz de tu solución.

**Crea un archivo `docker-compose.yml`:**

YAML

`services:
  rabbitmq:
    image: rabbitmq:3-management
    container_name: supermercado-broker
    ports:
      - "5672:5672"   # Puerto de comunicación (AMQP)
      - "15672:15672" # Puerto de la interfaz web (Dashboard)
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest
    networks:
      - supermercado-network

networks:
  supermercado-network:
    driver: bridge`

- **Para probarlo:** Ejecuta `docker-compose up -d` y entra a `http://localhost:15672`. Ahí verás cómo se mueven tus mensajes.

---

### 3. API Gateway (Con YARP)

Microsoft recomienda **YARP (Yet Another Reverse Proxy)** por su rendimiento y porque se configura 100% en C#.

**Pasos:**

1. Crea un proyecto **ASP.NET Core Empty** llamado `Supermercado.Gateway`.
2. Instala el paquete NuGet: `Yarp.ReverseProxy`.
3. En el archivo `appsettings.json`, define las rutas. Esto es lo que permite "ensamblar" módulos:

JSON

`{
  "ReverseProxy": {
    "Routes": {
      "catalog-route": {
        "ClusterId": "catalog-cluster",
        "Match": { "Path": "/api/catalog/{**remainder}" }
      },
      "sales-route": {
        "ClusterId": "sales-cluster",
        "Match": { "Path": "/api/sales/{**remainder}" }
      }
    },
    "Clusters": {
      "catalog-cluster": {
        "Destinations": {
          "destination1": { "Address": "http://catalog-service:8080" }
        }
      },
      "sales-cluster": {
        "Destinations": {
          "destination1": { "Address": "http://sales-service:8080" }
        }
      }
    }
  }
}`

1. En `Program.cs`, actívalo:

C#

`var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();
app.MapReverseProxy();
app.Run();`

---

### Resumen de la Fase 1: ¿Qué logramos?

- **Shared Kernel:** Si mañana creas el módulo de **Inventario**, solo importas esta librería y ya sabes cómo leer el evento `ProductCreated`.
- **RabbitMQ:** Tienes el "servidor de correos" listo para recibir mensajes.
- **Gateway:** Tu frontend de Next.js solo golpeará a `localhost:5000/api/catalog`. No le importa si el servicio de catálogo está en un contenedor o en otro país; el Gateway se encarga de encontrarlo.

# Arquitectura de microservicios:

**Objetivo**: diseñar un sistema para supermercado modular, escalable y comercializable como “piezas de Lego”, separando el modelo por **dominios de negocio** (no solo por tablas).

### 1) Mapa de entidades por dominio

| Dominio | Entidades principales | Atributos clave (ejemplos) |
| --- | --- | --- |
| **Catálogo (Maestro)** | Producto, Categoría, Marca, Unidad de medida | SKU, EAN (código de barras), Descripción, Imagen |
| **Inventario** | Stock, Bodega (Ubicación), Movimiento de inventario | Lote, Fecha de vencimiento, Cantidad, Alerta mínima |
| **Compras** | Proveedor, Orden de compra, Recepción de mercancía | NIT/TaxID, Precio de costo, Fecha de entrega |
| **Ventas (POS)** | Factura (cabecera), Detalle de venta, Sesión de caja | Número de factura, Impuestos (IVA), Método de pago |
| **Clientes** | Cliente, Programa de puntos, Historial de crédito | Documento de identidad, Puntos acumulados, Saldo |
| **Usuarios** | Usuario, Rol, Permiso | Email, Password (hashed), Nivel de acceso |

---

### 2) Microservicios sugeridos (arquitectura modular)

Separar en **6 microservicios** permite “ensamblar” la solución y ofrecer módulos premium (por ejemplo, *Inventario Pro*).

#### 2.1 Identity Service (IAM)

- **Responsabilidad:** autenticación y autorización (JWT).
- **Entidades:** Users, Roles.
- **Por qué separarlo:** suele ser el primer punto de entrada. Puede escalar el tráfico de login sin afectar ventas.

#### 2.2 Catalog Service

- **Responsabilidad:** el “diccionario” de lo que se vende.
- **Entidades:** Products, Categories.
- **Interacción:** emite el evento `ProductCreated` al registrar un producto.

#### 2.3 Inventory Service

- **Responsabilidad:** saber cuánto hay y dónde está.
- **Entidades:** Stock, Warehouse.
- **Interacción:** escucha a *Compras* para sumar y a *Ventas* para restar. Es clave para la consistencia.

#### 2.4 Procurement Service (Compras)

- **Responsabilidad:** relación con proveedores y entrada de mercancía.
- **Entidades:** Suppliers, PurchaseOrders.
- **Interacción:** al confirmar una recepción, emite `StockReceived`.

#### 2.5 Ordering / Sales Service (Ventas)

- **Responsabilidad:** checkout y generación de ticket.
- **Entidades:** Orders, Payments.
- **Interacción:** emite `SaleCompleted`.
- **Nota:** suele requerir la mayor disponibilidad (si cae, el supermercado no cobra).

#### 2.6 Customer / Loyalty Service

- **Responsabilidad:** fidelización y datos del cliente.
- **Entidades:** Customers, Points.
- **Interacción:** escucha `SaleCompleted` para calcular puntos.

---

### 3) Interacción cronológica (flujo de ejemplo)

1. **Catalog Service** registra “Cerveza Club Colombia” → emite `ProductCreated`.
2. **Inventory Service** recibe `ProductCreated` → crea stock inicial en `0`.
3. **Procurement Service** registra compra de 100 unidades → al recibir mercancía emite `StockReceived`.
4. **Inventory Service** recibe `StockReceived` → actualiza stock a `100`.
5. **Sales Service** vende 2 unidades → emite `SaleCompleted`.
6. **Inventory Service** recibe `SaleCompleted` → descuenta 2 (stock `98`).
7. **Customer Service** recibe `SaleCompleted` → suma puntos por la compra.

<aside>
✅

**Eventos clave**

- `ProductCreated`
- `StockReceived`
- `SaleCompleted`
</aside>

# Api Gateway configuración

Para configurar tu **API Gateway** con **YARP (Yet Another Reverse Proxy)** no necesitas un instalador externo. YARP se integra directamente en un proyecto de **.NET**.

En esta guía vas a crear el contenedor que funcionará como **puerta de entrada** para tus microservicios.

---

### 1) Crear el proyecto en .NET

Abre tu terminal en la carpeta raíz de tu solución y ejecuta:

```bash
# 1. Crear el proyecto web vacío
dotnet new web -n Supermercado.Gateway

# 2. Entrar a la carpeta
cd Supermercado.Gateway

# 3. Instalar el paquete de YARP
dotnet add package Yarp.ReverseProxy
```

---

### 2) Configurar el enrutamiento (appsettings.json)

YARP necesita saber a dónde enviar cada petición. Configuraremos rutas para los microservicios de **Catálogo** e **Identity**.

Edita `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ReverseProxy": {
    "Routes": {
      "catalog-route": {
        "ClusterId": "catalog-cluster",
        "Match": {
          "Path": "/api/catalog/{**remainder}"
        }
      },
      "identity-route": {
        "ClusterId": "identity-cluster",
        "Match": {
          "Path": "/api/auth/{**remainder}"
        }
      }
    },
    "Clusters": {
      "catalog-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "http://catalog-service:8080"
          }
        }
      },
      "identity-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "http://identity-service:8080"
          }
        }
      }
    }
  }
}
```

<aside>
💡

Usamos `catalog-service:8080` e `identity-service:8080` porque así se llamarán los contenedores dentro de la red de Docker.

</aside>

---

### 3) Activar YARP en el código (Program.cs)

Modifica `Program.cs` para registrar y mapear el reverse proxy:

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Agregar el servicio de YARP leyendo la configuración de appsettings
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// 2. Mapear el Reverse Proxy
app.MapReverseProxy();

app.Run();
```

---

### 4) Dockerización del gateway

Crea un archivo `Dockerfile` dentro de `Supermercado.Gateway`:

```docker
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Supermercado.Gateway.csproj", "./"]
RUN dotnet restore "Supermercado.Gateway.csproj"
COPY . .
RUN dotnet build "Supermercado.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Supermercado.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Supermercado.Gateway.dll"]
```

---

### 5) Integración en docker-compose.yml

Añade el gateway a tu `docker-compose.yml` (el mismo donde tienes RabbitMQ) para que todo viva en la misma red:

```yaml
services:
  gateway:
    image: gateway-img
    build:
      context: ./Supermercado.Gateway
    ports:
      - "5000:80" # Tu frontend llamará a http://localhost:5000
    networks:
      - supermercado-network

  # Aquí irán añadiéndose tus microservicios (catalog-service, identity-service, etc.)

networks:
  supermercado-network:
    driver: bridge
```

---

### ¿Qué acabas de configurar?

Ahora tienes un **punto único de entrada**.

- Si alguien pide [`http://localhost:5000/api/catalog/products`](http://localhost:5000/api/catalog/products), el gateway redirige internamente al contenedor de Catálogo.
- Esto te permite **ocultar tus microservicios** detrás de un firewall y exponer públicamente solo el gateway.

# RabbitMQ Configuración

**Objetivo:** levantar RabbitMQ con *Management UI* usando **Docker** (sin instalar nada localmente) para que tus microservicios **.NET** se conecten por AMQP.

---

### 1) Actualizar `docker-compose.yml`

RabbitMQ necesita un panel de administración para inspeccionar colas, exchanges y mensajes. Para eso usaremos la imagen oficial con el tag `-management`.

Pega este servicio en tu `docker-compose.yml` (junto al Gateway si ya lo tienes):

```yaml
services:
  rabbitmq:
    image: rabbitmq:3-management
    container_name: supermercado-broker
    ports:
      - "5672:5672"   # Puerto para comunicación de microservicios (AMQP)
      - "15672:15672" # Puerto para el Panel Web (Management UI)
    environment:
      - RABBITMQ_DEFAULT_USER=admin
      - RABBITMQ_DEFAULT_PASS=p@ssword123  # Cámbialo en producción
    networks:
      - supermercado-network

networks:
  supermercado-network:
    driver: bridge
```

<aside>
🔒

En producción, **no** uses credenciales hardcodeadas. Usa variables de entorno seguras o un secret manager.

</aside>

---

### 2) Levantar el servicio

Abre tu terminal en la carpeta donde está el archivo y ejecuta:

```bash
docker-compose up -d rabbitmq
```

---

### 3) Acceder al panel de control

Una vez levantado, abre en tu navegador:

- **URL:** `http://localhost:15672`
- **Username:** `admin`
- **Password:** `p@ssword123`

Desde aquí podrás ver:

- **Exchanges:** donde llegan/publican los eventos.
- **Queues:** donde se almacenan para ser consumidos.

---

### 4) Preparar tus microservicios (.NET + MassTransit)

Para que tus microservicios en C# hablen con RabbitMQ de forma sencilla, usaremos **MassTransit**. Esto abstrae la complejidad del broker y te permite trabajar con tipos de C#.

#### Paso A: instalar librerías

Instala este paquete en cada microservicio que vaya a publicar o consumir eventos (por ejemplo, Catálogo, Ventas, Inventario):

```bash
dotnet add package MassTransit.RabbitMQ
```

#### Paso B: configurar `Program.cs`

Ejemplo (microservicio **Catalog**). Nota: `rabbitmq` es el nombre del servicio en el `docker-compose`:

```csharp
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("admin");
            h.Password("p@ssword123");
        });

        cfg.ConfigureEndpoints(context);
    });
});
```

---

### Conceptos clave para tu sistema de facturación

- **Exchange:** como la “oficina de correos”. Cuando el microservicio de **Catálogo** crea un producto, publica el evento en un exchange.
- **Queue (Cola):** como el “buzón”. El microservicio de **Inventario** tendrá su propia cola conectada al exchange para recibir los mensajes que le interesan.
- **Durabilidad de mensajes:** RabbitMQ puede persistir mensajes. Si **Inventario** está apagado, el mensaje queda en la cola hasta que vuelva a encender. Esto ayuda a que no se pierdan actualizaciones de stock.