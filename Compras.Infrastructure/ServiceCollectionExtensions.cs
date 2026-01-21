using Compras.Application.Interfaces;
using Compras.Infrastructure.Repositories;
using Compras.Infrastructure.Time;
using Microsoft.Extensions.DependencyInjection;

namespace Compras.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPurchaseRepository, InMemoryPurchaseRepository>();
        services.AddSingleton<IDateTimeProvider, SystemClock>();
        return services;
    }
}
