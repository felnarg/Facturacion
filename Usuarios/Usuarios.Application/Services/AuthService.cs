using System.Security.Cryptography;
using System.Text;
using Usuarios.Application.Abstractions;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Constants;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Repositories;

namespace Usuarios.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _repository;
    private readonly IRoleRepository _roleRepository;
    private readonly IJwtTokenService _tokenService;

    public AuthService(
        IUserRepository repository, 
        IRoleRepository roleRepository,
        IJwtTokenService tokenService)
    {
        _repository = repository;
        _roleRepository = roleRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("El email ya esta registrado.");
        }

        var passwordHash = ComputeHash(request.Password);
        var user = new User(request.Name, request.Email, passwordHash);
        
        // Asignar rol de cliente por defecto
        var customerRole = await _roleRepository.GetByCodeAsync(RoleCodes.Customer, cancellationToken);
        if (customerRole is not null)
        {
            var userRole = new UserRole(user.Id, customerRole.Id);
            user.AddRole(userRole);
        }
        
        await _repository.AddAsync(user, cancellationToken);

        // Generar token con roles y permisos
        var roles = user.GetActiveRoleCodes().ToList();
        var permissions = user.GetPermissionCodes().ToList();
        var token = _tokenService.GenerateToken(user.Id, user.Email, user.Name, roles, permissions);
        
        return new AuthResponse(user.Id, user.Email, user.Name, token, roles, permissions);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByEmailWithRolesAndPermissionsAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return null;
        }

        // Verificar si el usuario está bloqueado
        if (user.IsLockedOut())
        {
            throw new InvalidOperationException($"La cuenta está bloqueada. Intente nuevamente después.");
        }

        // Verificar si el usuario está activo
        if (!user.IsActive)
        {
            throw new InvalidOperationException("La cuenta está desactivada. Contacte al administrador.");
        }

        var passwordHash = ComputeHash(request.Password);
        if (!string.Equals(user.PasswordHash, passwordHash, StringComparison.Ordinal))
        {
            // Registrar intento fallido
            user.RecordFailedLogin();
            await _repository.UpdateAsync(user, cancellationToken);
            return null;
        }

        // Registrar login exitoso
        user.RecordSuccessfulLogin();
        await _repository.UpdateAsync(user, cancellationToken);

        // Generar token con roles y permisos
        var roles = user.GetActiveRoleCodes().ToList();
        var permissions = user.GetPermissionCodes().ToList();
        var token = _tokenService.GenerateToken(user.Id, user.Email, user.Name, roles, permissions);
        
        return new AuthResponse(user.Id, user.Email, user.Name, token, roles, permissions);
    }

    private static string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}

