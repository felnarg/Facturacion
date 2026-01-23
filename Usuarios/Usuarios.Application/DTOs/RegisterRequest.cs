namespace Usuarios.Application.DTOs;

public sealed record RegisterRequest(string Name, string Email, string Password);
