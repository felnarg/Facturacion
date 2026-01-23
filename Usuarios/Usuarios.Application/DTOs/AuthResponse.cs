namespace Usuarios.Application.DTOs;

public sealed record AuthResponse(Guid UserId, string Email, string Token);
