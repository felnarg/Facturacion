using Kardex.Application.Abstractions;
using Kardex.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kardex.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreditAccountService, CreditAccountService>();
        services.AddScoped<ICreditSaleService, CreditSaleService>();
        return services;
    }
}
