using Microsoft.Extensions.DependencyInjection;
using Usuarios.Application.Abstractions;
using Usuarios.Application.Services;

namespace Usuarios.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Servicios de usuarios
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        
        // Servicios RBAC
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        
        return services;
    }
}

