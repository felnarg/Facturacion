using Compras.Domain.Exceptions;

namespace Compras.Domain.ValueObjects;

public sealed class Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new DomainException("El monto no puede ser negativo.");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException("La moneda es obligatoria.");
        }

        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new DomainException("La moneda debe ser la misma para sumar.");
        }

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(int factor)
    {
        if (factor <= 0)
        {
            throw new DomainException("La cantidad debe ser mayor que cero.");
        }

        return new Money(Amount * factor, Currency);
    }
}
