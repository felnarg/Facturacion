namespace Clientes.Domain.Entities;

public enum CustomerType
{
    Natural = 1,
    Juridica = 2
}

public enum IdentificationType
{
    CC = 1,
    NIT = 2
}

public class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public CustomerType Type { get; private set; }
    public IdentificationType IdentificationType { get; private set; }
    public string IdentificationNumber { get; private set; } = string.Empty;
    public bool IsCreditApproved { get; private set; }
    public decimal ApprovedCreditLimit { get; private set; }
    public int ApprovedPaymentTermDays { get; private set; }
    public int Points { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Customer()
    {
    }

    public Customer(
        string name,
        string email,
        string city,
        string phone,
        string address,
        CustomerType type,
        IdentificationType identificationType,
        string identificationNumber,
        bool isCreditApproved = false,
        decimal approvedCreditLimit = 0,
        int approvedPaymentTermDays = 0)
    {
        ValidateRequired(name, "El nombre es obligatorio.", nameof(name));
        ValidateRequired(email, "El email es obligatorio.", nameof(email));
        ValidateRequired(city, "La ciudad es obligatoria.", nameof(city));
        ValidateRequired(phone, "El teléfono es obligatorio.", nameof(phone));
        ValidateRequired(address, "La dirección es obligatoria.", nameof(address));
        ValidateRequired(identificationNumber, "El número de identificación es obligatorio.", nameof(identificationNumber));

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = email.Trim();
        City = city.Trim();
        Phone = phone.Trim();
        Address = address.Trim();
        Type = type;
        IdentificationType = identificationType;
        IdentificationNumber = identificationNumber.Trim();
        SetCreditApproval(isCreditApproved, approvedCreditLimit, approvedPaymentTermDays);
        Points = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void UpdateDetails(
        string name,
        string email,
        string city,
        string phone,
        string address,
        CustomerType type,
        IdentificationType identificationType,
        string identificationNumber)
    {
        ValidateRequired(name, "El nombre es obligatorio.", nameof(name));
        ValidateRequired(email, "El email es obligatorio.", nameof(email));
        ValidateRequired(city, "La ciudad es obligatoria.", nameof(city));
        ValidateRequired(phone, "El teléfono es obligatorio.", nameof(phone));
        ValidateRequired(address, "La dirección es obligatoria.", nameof(address));
        ValidateRequired(identificationNumber, "El número de identificación es obligatorio.", nameof(identificationNumber));

        Name = name.Trim();
        Email = email.Trim();
        City = city.Trim();
        Phone = phone.Trim();
        Address = address.Trim();
        Type = type;
        IdentificationType = identificationType;
        IdentificationNumber = identificationNumber.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCreditApproval(bool isApproved, decimal approvedCreditLimit, int approvedPaymentTermDays)
    {
        if (isApproved)
        {
            if (approvedCreditLimit <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(approvedCreditLimit), "El cupo aprobado debe ser mayor a cero.");
            }

            if (approvedPaymentTermDays <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(approvedPaymentTermDays), "Los días permitidos deben ser mayores a cero.");
            }
        }

        IsCreditApproved = isApproved;
        ApprovedCreditLimit = isApproved ? decimal.Round(approvedCreditLimit, 2) : 0;
        ApprovedPaymentTermDays = isApproved ? approvedPaymentTermDays : 0;
        UpdatedAt = DateTime.UtcNow;
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

    private static void ValidateRequired(string value, string message, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, fieldName);
        }
    }
}
