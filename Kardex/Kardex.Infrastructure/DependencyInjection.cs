using Kardex.Domain.Repositories;
using Kardex.Infrastructure.Data;
using Kardex.Infrastructure.Messaging;
using Kardex.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kardex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<KardexDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly("Kardex.API")));

        services.AddScoped<ICreditAccountRepository, CreditAccountRepository>();
        services.AddScoped<ICreditMovementRepository, CreditMovementRepository>();

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.AddHostedService<CreditSaleRequestedConsumer>();
        services.AddHostedService<CustomerCreditApprovedConsumer>();

        return services;
    }
}
