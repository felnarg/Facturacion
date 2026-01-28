namespace Usuarios.Domain.Entities;

/// <summary>
/// Entidad de unión entre Usuario y Rol (relación muchos a muchos)
/// </summary>
public class UserRole
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;
    
    /// <summary>
    /// Fecha de asignación del rol
    /// </summary>
    public DateTime AssignedAt { get; private set; }
    
    /// <summary>
    /// Usuario que asignó el rol (para auditoría)
    /// </summary>
    public Guid? AssignedBy { get; private set; }
    
    /// <summary>
    /// Fecha de expiración del rol (opcional, para roles temporales)
    /// </summary>
    public DateTime? ExpiresAt { get; private set; }

    private UserRole()
    {
    }

    public UserRole(Guid userId, Guid roleId, Guid? assignedBy = null, DateTime? expiresAt = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired() => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;

    public void SetExpiration(DateTime? expiresAt)
    {
        ExpiresAt = expiresAt;
    }
}
