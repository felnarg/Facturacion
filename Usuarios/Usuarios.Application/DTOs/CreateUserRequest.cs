namespace Usuarios.Application.DTOs;

public sealed record CreateUserRequest(string Name, string Email, string Password);
