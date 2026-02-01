using Kardex.Domain.Enums;

namespace Kardex.Domain.Entities;

public class CreditAccount
{
    private readonly List<CreditMovement> _movements = new();

    public Guid Id { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public IdentificationType IdentificationType { get; private set; }
    public string IdentificationNumber { get; private set; } = string.Empty;
    public decimal CreditLimit { get; private set; }
    public int PaymentTermDays { get; private set; }
    public decimal CurrentBalance { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<CreditMovement> Movements => _movements.AsReadOnly();

    private CreditAccount()
    {
    }

    public CreditAccount(
        string customerName,
        IdentificationType identificationType,
        string identificationNumber,
        decimal creditLimit,
        int paymentTermDays)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("El nombre del cliente o empresa es obligatorio.", nameof(customerName));
        }

        if (string.IsNullOrWhiteSpace(identificationNumber))
        {
            throw new ArgumentException("La identificación es obligatoria.", nameof(identificationNumber));
        }

        if (creditLimit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(creditLimit), "El cupo debe ser mayor a cero.");
        }

        if (paymentTermDays <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(paymentTermDays), "El período de pago debe ser mayor a cero.");
        }

        Id = Guid.NewGuid();
        CustomerName = customerName.Trim();
        IdentificationType = identificationType;
        IdentificationNumber = identificationNumber.Trim();
        CreditLimit = decimal.Round(creditLimit, 2);
        PaymentTermDays = paymentTermDays;
        CurrentBalance = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public decimal AvailableCredit => CreditLimit - CurrentBalance;

    public void UpdateDetails(string customerName, decimal creditLimit, int paymentTermDays)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("El nombre del cliente o empresa es obligatorio.", nameof(customerName));
        }

        if (creditLimit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(creditLimit), "El cupo debe ser mayor a cero.");
        }

        if (paymentTermDays <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(paymentTermDays), "El período de pago debe ser mayor a cero.");
        }

        CustomerName = customerName.Trim();
        CreditLimit = decimal.Round(creditLimit, 2);
        PaymentTermDays = paymentTermDays;
        UpdatedAt = DateTime.UtcNow;
    }

    public CreditMovement RegisterCreditSale(decimal amount, Guid? saleId, DateTime? occurredOn = null)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "El monto del crédito debe ser mayor a cero.");
        }

        if (amount > AvailableCredit)
        {
            throw new InvalidOperationException("El cliente no tiene cupo suficiente para esta venta.");
        }

        var baseDate = occurredOn ?? DateTime.UtcNow;
        var dueDate = baseDate.Date.AddDays(PaymentTermDays);

        var movement = new CreditMovement(Id, saleId, amount, dueDate);
        _movements.Add(movement);

        CurrentBalance = decimal.Round(CurrentBalance + amount, 2);
        UpdatedAt = DateTime.UtcNow;

        return movement;
    }
}
