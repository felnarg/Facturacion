using Compras.Domain.Entities;

namespace Compras.Infrastructure.Data.Seed;

public static class SeedSuppliers
{
    public static IReadOnlyList<object> Get()
    {
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return new List<object>
        {
            new
            {
                Id = Guid.Parse("7b91c222-62cb-4e1b-8c1b-0f3cf01a1a11"),
                Name = "Distribuidora Central",
                ContactName = "Maria Lopez",
                Phone = "+57 300 111 2233",
                Email = "maria@distribuidoracentral.com",
                Address = "Calle 10 # 15-20",
                CreatedAt = createdAt
            },
            new
            {
                Id = Guid.Parse("e12f64b9-0b0b-4f7f-9b2b-9e9d4d2a2a22"),
                Name = "Proveedores del Norte",
                ContactName = "Juan Perez",
                Phone = "+57 300 222 3344",
                Email = "juan@proveedoresnorte.com",
                Address = "Carrera 45 # 22-10",
                CreatedAt = createdAt
            },
            new
            {
                Id = Guid.Parse("3b99c56f-9a9a-4f2f-8c4c-8f31b3c3c333"),
                Name = "Alimentos Premium",
                ContactName = "Luisa Gomez",
                Phone = "+57 300 333 4455",
                Email = "luisa@alimentos-premium.com",
                Address = "Av. 80 # 40-30",
                CreatedAt = createdAt
            }
        };
    }
}
