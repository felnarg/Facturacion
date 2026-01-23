using Compras.Application.Abstractions;
using Compras.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Compras.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPurchaseService, PurchaseService>();
        return services;
    }
}
