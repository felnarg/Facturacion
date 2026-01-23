namespace Clientes.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public int Points { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Customer()
    {
    }

    public Customer(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre es obligatorio.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email es obligatorio.", nameof(email));
        }

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = email.Trim();
        Points = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void AddPoints(int points)
    {
        if (points <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(points), "Los puntos deben ser mayores a cero.");
        }

        Points += points;
        UpdatedAt = DateTime.UtcNow;
    }
}
