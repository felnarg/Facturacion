using Microsoft.Extensions.DependencyInjection;
using Ventas.Application.Abstractions;
using Ventas.Application.Services;

namespace Ventas.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISaleService, SaleService>();
        return services;
    }
}
