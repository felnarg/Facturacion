using Clientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Infrastructure.Data.Seed;

public static class SeedCustomers
{
    public static async Task SeedAsync(ClientesDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Customers.AnyAsync(cancellationToken))
        {
            return;
        }

        var customers = new List<Customer>
        {
            new(
                "Juan Perez",
                "juan.perez@empresa.com",
                "Bogotá",
                "3001234567",
                "Calle 10 # 20-30",
                CustomerType.Natural,
                IdentificationType.CC,
                "1234567890"),
            new(
                "Distribuciones Andina",
                "contacto@andina.com",
                "Medellín",
                "6041234567",
                "Carrera 45 # 30-12",
                CustomerType.Juridica,
                IdentificationType.NIT,
                "900123456-7")
        };

        dbContext.Customers.AddRange(customers);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
