using Compras.Application.Interfaces;
using Compras.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Compras.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPurchaseService, PurchaseService>();
        return services;
    }
}
