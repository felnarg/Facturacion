using Catalogo.Application.Abstractions;
using Catalogo.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Catalogo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        return services;
    }
}
