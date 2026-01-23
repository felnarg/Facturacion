using Clientes.Application.Abstractions;
using Clientes.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Clientes.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISaleHistoryService, SaleHistoryService>();
        return services;
    }
}
