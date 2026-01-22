namespace Catalogo.Application.DTOs;

public sealed record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string Sku);
