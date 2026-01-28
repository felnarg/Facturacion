namespace Usuarios.Domain.Entities;

/// <summary>
/// Entidad de unión entre Rol y Permiso (relación muchos a muchos)
/// </summary>
public class RolePermission
{
    public Guid Id { get; private set; }
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;
    public Guid PermissionId { get; private set; }
    public Permission Permission { get; private set; } = null!;
    
    /// <summary>
    /// Fecha de asignación del permiso al rol
    /// </summary>
    public DateTime AssignedAt { get; private set; }

    private RolePermission()
    {
    }

    public RolePermission(Guid roleId, Guid permissionId)
    {
        Id = Guid.NewGuid();
        RoleId = roleId;
        PermissionId = permissionId;
        AssignedAt = DateTime.UtcNow;
    }
}
