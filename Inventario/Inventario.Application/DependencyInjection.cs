using Inventario.Application.Abstractions;
using Inventario.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inventario.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IStockService, StockService>();
        return services;
    }
}
