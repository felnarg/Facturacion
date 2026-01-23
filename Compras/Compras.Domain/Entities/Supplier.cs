namespace Compras.Domain.Entities;

public class Supplier
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ContactName { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private Supplier()
    {
    }

    public Supplier(string name, string contactName, string phone, string email, string address)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del proveedor es obligatorio.", nameof(name));
        }

        Id = Guid.NewGuid();
        Name = name.Trim();
        ContactName = contactName?.Trim() ?? string.Empty;
        Phone = phone?.Trim() ?? string.Empty;
        Email = email?.Trim() ?? string.Empty;
        Address = address?.Trim() ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
    }
}
