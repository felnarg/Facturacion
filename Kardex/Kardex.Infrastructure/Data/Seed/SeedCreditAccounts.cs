using Kardex.Domain.Enums;

namespace Kardex.Infrastructure.Data.Seed;

public static class SeedCreditAccounts
{
    public static IReadOnlyList<object> Get()
    {
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return new List<object>
        {
            new
            {
                Id = Guid.Parse("b10efb7e-1d4a-4b55-9f45-1f3f9b42a111"),
                CustomerName = "Comercializadora La 45",
                IdentificationType = IdentificationType.NIT,
                IdentificationNumber = "900123456-7",
                CreditLimit = 5000000m,
                PaymentTermDays = 30,
                CurrentBalance = 0m,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },
            new
            {
                Id = Guid.Parse("a2ff7e2b-c7f2-4a1b-b0b7-7771e3a2b222"),
                CustomerName = "Carlos Medina",
                IdentificationType = IdentificationType.CC,
                IdentificationNumber = "1030123456",
                CreditLimit = 1200000m,
                PaymentTermDays = 20,
                CurrentBalance = 0m,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            }
        };
    }
}
