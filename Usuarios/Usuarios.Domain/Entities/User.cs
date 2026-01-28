namespace Usuarios.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    /// <summary>
    /// Indica si el usuario está activo en el sistema
    /// </summary>
    public bool IsActive { get; private set; } = true;
    
    /// <summary>
    /// Fecha del último inicio de sesión
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }
    
    /// <summary>
    /// Número de teléfono del usuario
    /// </summary>
    public string? PhoneNumber { get; private set; }
    
    /// <summary>
    /// URL de la foto de perfil del usuario
    /// </summary>
    public string? ProfilePictureUrl { get; private set; }
    
    /// <summary>
    /// Fecha de bloqueo de la cuenta (si está bloqueada)
    /// </summary>
    public DateTime? LockedUntil { get; private set; }
    
    /// <summary>
    /// Número de intentos de inicio de sesión fallidos
    /// </summary>
    public int FailedLoginAttempts { get; private set; }

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private User()
    {
    }

    public User(string name, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre es obligatorio.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email es obligatorio.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("La contraseña es obligatoria.", nameof(passwordHash));
        }

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash.Trim();
        IsActive = true;
        FailedLoginAttempts = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void Update(string? name, string? phoneNumber, string? profilePictureUrl)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name.Trim();
        
        PhoneNumber = phoneNumber?.Trim();
        ProfilePictureUrl = profilePictureUrl?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("La contraseña es obligatoria.", nameof(newPasswordHash));
        
        PasswordHash = newPasswordHash.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordSuccessfulLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockedUntil = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordFailedLogin(int maxAttempts = 5, int lockoutMinutes = 30)
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= maxAttempts)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(lockoutMinutes);
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsLockedOut() => LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unlock()
    {
        LockedUntil = null;
        FailedLoginAttempts = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRole(UserRole userRole)
    {
        if (!_userRoles.Any(ur => ur.RoleId == userRole.RoleId))
        {
            _userRoles.Add(userRole);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveRole(Guid roleId)
    {
        var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
        if (userRole is not null)
        {
            _userRoles.Remove(userRole);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void ClearRoles()
    {
        _userRoles.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public IEnumerable<string> GetActiveRoleCodes()
    {
        return _userRoles
            .Where(ur => !ur.IsExpired() && ur.Role.IsActive)
            .Select(ur => ur.Role.Code);
    }

    public IEnumerable<string> GetPermissionCodes()
    {
        return _userRoles
            .Where(ur => !ur.IsExpired() && ur.Role.IsActive)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct();
    }
}
