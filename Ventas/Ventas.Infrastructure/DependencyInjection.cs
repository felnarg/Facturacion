using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ventas.Application.Abstractions;
using Ventas.Domain.Repositories;
using Ventas.Infrastructure.Data;
using Ventas.Infrastructure.Messaging;
using Ventas.Infrastructure.Repositories;

namespace Ventas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<VentasDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly("Ventas.API")));

        services.AddScoped<ISaleRepository, SaleRepository>();

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton<IEventBus, RabbitMqEventBus>();

        return services;
    }
}
