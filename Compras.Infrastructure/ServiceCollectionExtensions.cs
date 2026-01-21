using Compras.Application.Interfaces;
using Compras.Infrastructure.Data;
using Compras.Infrastructure.Repositories;
using Compras.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Compras.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        string? migrationsAssembly = null)
    {
        services.AddDbContext<ComprasDbContext>(options =>
        {
            if (string.IsNullOrWhiteSpace(migrationsAssembly))
            {
                options.UseSqlServer(connectionString);
                return;
            }

            options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
        });

        services.AddScoped<IPurchaseRepository, PurchaseRepository>();
        services.AddSingleton<IDateTimeProvider, SystemClock>();
        return services;
    }
}
