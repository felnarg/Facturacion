using Clientes.Application.Abstractions;
using Clientes.Domain.Repositories;
using Clientes.Infrastructure.Data;
using Clientes.Infrastructure.Messaging;
using Clientes.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clientes.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ClientesDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly("Clientes.API")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ISaleHistoryRepository, SaleHistoryRepository>();

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.AddHostedService<SaleCompletedConsumer>();

        return services;
    }
}
