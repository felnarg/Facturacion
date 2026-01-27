namespace Inventario.Domain.Entities;

public enum StockMovementType
{
    Sale,
    Purchase,
    Adjustment,
    Return,
    Creation
}

public class StockMovement
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public StockMovementType Type { get; private set; }
    public int Quantity { get; private set; }
    public Guid? ReferenceId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private StockMovement() { }

    public StockMovement(Guid productId, StockMovementType type, int quantity, Guid? referenceId)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Type = type;
        Quantity = quantity;
        ReferenceId = referenceId;
        CreatedAt = DateTime.UtcNow;
    }
}
