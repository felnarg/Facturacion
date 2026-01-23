namespace Clientes.Domain.Entities;

public class SaleHistory
{
    public Guid Id { get; private set; }
    public Guid SaleId { get; private set; }
    public int ItemsCount { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private SaleHistory()
    {
    }

    public SaleHistory(Guid saleId, int itemsCount, DateTime occurredAt)
    {
        if (itemsCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(itemsCount), "La cantidad de items debe ser mayor a cero.");
        }

        Id = Guid.NewGuid();
        SaleId = saleId;
        ItemsCount = itemsCount;
        OccurredAt = occurredAt;
    }
}
