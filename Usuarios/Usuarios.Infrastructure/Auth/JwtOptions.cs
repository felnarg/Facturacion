namespace Usuarios.Infrastructure.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "Facturacion.IAM";
    public string Audience { get; set; } = "Facturacion.Clients";
    public string Key { get; set; } = "change-me-in-production";
    public int ExpirationMinutes { get; set; } = 60;
}
