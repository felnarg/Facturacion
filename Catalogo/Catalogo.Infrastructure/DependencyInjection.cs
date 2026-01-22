using Catalogo.Application.Abstractions;
using Catalogo.Domain.Repositories;
using Catalogo.Infrastructure.Data;
using Catalogo.Infrastructure.Messaging;
using Catalogo.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalogo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogoDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly("Catalogo.API")));

        services.AddScoped<IProductRepository, ProductRepository>();

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton<IEventBus, RabbitMqEventBus>();

        return services;
    }
}
