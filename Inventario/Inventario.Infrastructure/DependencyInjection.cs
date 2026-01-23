using Inventario.Application.Abstractions;
using Inventario.Domain.Repositories;
using Inventario.Infrastructure.Data;
using Inventario.Infrastructure.Messaging;
using Inventario.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventario.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<InventarioDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly("Inventario.API")));

        services.AddScoped<IStockRepository, StockRepository>();

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.AddHostedService<InventoryEventsConsumer>();

        return services;
    }
}
