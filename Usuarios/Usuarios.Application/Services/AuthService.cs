using System.Security.Cryptography;
using System.Text;
using Usuarios.Application.Abstractions;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Repositories;

namespace Usuarios.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _repository;
    private readonly IJwtTokenService _tokenService;

    public AuthService(IUserRepository repository, IJwtTokenService tokenService)
    {
        _repository = repository;
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
        await _repository.AddAsync(user, cancellationToken);

        var token = _tokenService.GenerateToken(user.Id, user.Email);
        return new AuthResponse(user.Id, user.Email, token);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return null;
        }

        var passwordHash = ComputeHash(request.Password);
        if (!string.Equals(user.PasswordHash, passwordHash, StringComparison.Ordinal))
        {
            return null;
        }

        var token = _tokenService.GenerateToken(user.Id, user.Email);
        return new AuthResponse(user.Id, user.Email, token);
    }

    private static string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}
