using Microsoft.Extensions.DependencyInjection;
using Usuarios.Application.Abstractions;
using Usuarios.Application.Services;

namespace Usuarios.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
