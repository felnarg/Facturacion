using Kardex.Domain.Enums;

namespace Kardex.Domain.Entities;

public class CreditMovement
{
    public Guid Id { get; private set; }
    public Guid CreditAccountId { get; private set; }
    public Guid? SaleId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime DueDate { get; private set; }
    public CreditMovementStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private CreditMovement()
    {
    }

    public CreditMovement(Guid creditAccountId, Guid? saleId, decimal amount, DateTime dueDate)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "El monto del crédito debe ser mayor a cero.");
        }

        if (dueDate <= DateTime.UtcNow.Date)
        {
            throw new ArgumentOutOfRangeException(nameof(dueDate), "La fecha de vencimiento debe ser futura.");
        }

        Id = Guid.NewGuid();
        CreditAccountId = creditAccountId;
        SaleId = saleId;
        Amount = decimal.Round(amount, 2);
        DueDate = dueDate;
        Status = CreditMovementStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkPaid()
    {
        Status = CreditMovementStatus.Paid;
    }
}
