namespace Catalogo.Application.DTOs;

public sealed record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string Sku);
