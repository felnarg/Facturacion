using Inventario.Domain.Entities;

namespace Inventario.Infrastructure.Data.Seed;

public static class SeedStocks
{
    private static readonly Guid Product1Id = Guid.Parse("2b6d7b3f-7c3c-4f6d-a9f5-7c5b5b9e0f01");
    private static readonly Guid Product2Id = Guid.Parse("9a2e8f9a-6d6d-4b6a-9c5f-1f8b6f3e4e02");
    
    // We cannot instantiate Stock/StockMovement easily because constructors are private/internal or have specific logic.
    // However, EF Core's HasData accepts anonymous objects to bypass constructors.
    
    public static IEnumerable<object> GetStocks()
    {
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        return new List<object>
        {
            new 
            {
                Id = Guid.Parse("d34e9a8b-1111-4444-8888-c75b5b9e0f01"),
                ProductId = Product1Id,
                Quantity = 100,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },
            new 
            {
                Id = Guid.Parse("e56f8b9c-2222-4444-8888-1f8b6f3e4e02"),
                ProductId = Product2Id,
                Quantity = 50,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            }
        };
    }

    public static IEnumerable<object> GetMovements()
    {
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return new List<object>
        {
            new 
            {
                Id = Guid.Parse("f67a8b9c-3333-4444-8888-c75b5b9e0f01"),
                ProductId = Product1Id,
                Type = StockMovementType.Creation,
                Quantity = 100,
                ReferenceId = (Guid?)null,
                CreatedAt = createdAt
            },
            new 
            {
                Id = Guid.Parse("a89b7c6d-4444-4444-8888-1f8b6f3e4e02"),
                ProductId = Product2Id,
                Type = StockMovementType.Creation,
                Quantity = 50,
                ReferenceId = (Guid?)null,
                CreatedAt = createdAt
            }
        };
    }
}
