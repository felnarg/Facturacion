# Documentación de Endpoints

Este documento describe los endpoints disponibles por microservicio y una
breve explicación de su comportamiento. Todas las rutas están expuestas bajo
`/api/...` y se consumen a través del API Gateway.

---

## Catálogo (`Catalogo.API`)
Base: `/api/catalog`

- `GET /api/catalog/products`
  - Lista todos los productos. Soporta `?search=...` para filtrar por texto.
- `GET /api/catalog/products/{id}`
  - Obtiene un producto por `Id`.
- `POST /api/catalog/products`
  - Crea un producto nuevo.
- `PUT /api/catalog/products/{id}`
  - Actualiza los datos de un producto existente.
- `PATCH /api/catalog/products/{id}/sale-percentage`
  - Actualiza solo el porcentaje de venta del producto.
- `DELETE /api/catalog/products/{id}`
  - Elimina un producto.

---

## Compras (`Compras.API`)
Base: `/api/purchases`

- `GET /api/purchases`
  - Lista todas las compras.
- `GET /api/purchases/{id}`
  - Obtiene una compra por `Id`.
- `POST /api/purchases`
  - Registra una compra nueva.
- `PUT /api/purchases/{id}/receive`
  - Marca una compra como recibida.

### Proveedores (`Compras.API`)
Base: `/api/suppliers`

- `GET /api/suppliers`
  - Lista proveedores. Soporta `?search=...` para filtrar por texto.
- `GET /api/suppliers/{id}`
  - Obtiene un proveedor por `Id`.
- `POST /api/suppliers`
  - Crea un proveedor nuevo.
- `PUT /api/suppliers/{id}`
  - Actualiza un proveedor existente.
- `DELETE /api/suppliers/{id}`
  - Elimina un proveedor.

---

## Inventario (`Inventario.API`)
Base: `/api/inventory`

- `GET /api/inventory/stocks`
  - Lista el stock de todos los productos.
- `GET /api/inventory/stocks/{productId}`
  - Obtiene el stock por `productId`.
- `POST /api/inventory/stocks/{productId}`
  - Crea el registro de stock para un producto.
- `PUT /api/inventory/stocks/{productId}/increase`
  - Incrementa stock (`{ quantity }`).
- `PUT /api/inventory/stocks/{productId}/decrease`
  - Disminuye stock (`{ quantity }`).

---

## Ventas (`Ventas.API`)
Base: `/api/sales`

- `GET /api/sales`
  - Lista todas las ventas.
- `GET /api/sales/{id}`
  - Obtiene una venta por `Id`.
- `POST /api/sales`
  - Registra una venta nueva.

---

## Clientes (`Clientes.API`)
Base: `/api/customers`

- `GET /api/customers`
  - Lista todos los clientes.
- `GET /api/customers/{id}`
  - Obtiene un cliente por `Id`.
- `POST /api/customers`
  - Crea un cliente nuevo.

---

## Usuarios / IAM (`Usuarios.API`)
Base: `/api/auth` y `/api/users`

- `POST /api/auth/register`
  - Registra un usuario y retorna tokens.
- `POST /api/auth/login`
  - Inicia sesión y retorna tokens.
- `GET /api/users`
  - Lista usuarios registrados.
- `GET /api/users/{id}`
  - Obtiene un usuario por `Id`.
- `POST /api/users`
  - Crea un usuario manualmente.

