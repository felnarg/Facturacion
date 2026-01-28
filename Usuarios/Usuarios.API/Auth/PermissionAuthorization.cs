using Microsoft.AspNetCore.Authorization;

namespace Usuarios.API.Auth;

/// <summary>
/// Requirement que indica que el usuario debe tener un permiso específico
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

/// <summary>
/// Handler que verifica si el usuario tiene el permiso requerido en sus claims
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Verificar si el usuario tiene el permiso requerido
        var permissions = context.User.FindAll("permissions").Select(c => c.Value);
        
        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        // También verificar si es SuperAdmin (tiene todos los permisos)
        var roles = context.User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value);
        if (roles.Contains("superadmin"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Provider que genera políticas de autorización dinámicamente basadas en permisos
/// </summary>
public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private const string PolicyPrefix = "RequirePermission:";
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public PermissionPolicyProvider(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AuthorizationOptions>>();
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName[PolicyPrefix.Length..];
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(permission))
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackPolicyProvider.GetFallbackPolicyAsync();
}
