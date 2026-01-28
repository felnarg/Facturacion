namespace Usuarios.Domain.Entities;

/// <summary>
/// Representa un rol en el sistema RBAC.
/// Los roles agrupan permisos y se asignan a usuarios.
/// </summary>
public class Role
{
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Código único del rol (e.g., "superadmin", "cashier")
    /// </summary>
    public string Code { get; private set; } = string.Empty;
    
    /// <summary>
    /// Nombre descriptivo del rol
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// Descripción del rol y sus responsabilidades
    /// </summary>
    public string Description { get; private set; } = string.Empty;
    
    /// <summary>
    /// Nivel jerárquico del rol (mayor = más privilegios)
    /// </summary>
    public int HierarchyLevel { get; private set; }
    
    /// <summary>
    /// Indica si el rol está activo
    /// </summary>
    public bool IsActive { get; private set; } = true;
    
    /// <summary>
    /// Indica si es un rol del sistema (no puede eliminarse)
    /// </summary>
    public bool IsSystemRole { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    private Role()
    {
    }

    public Role(string code, string name, string description, int hierarchyLevel, bool isSystemRole = false)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("El código del rol es obligatorio.", nameof(code));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del rol es obligatorio.", nameof(name));

        Id = Guid.NewGuid();
        Code = code.Trim().ToLowerInvariant();
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        HierarchyLevel = hierarchyLevel;
        IsSystemRole = isSystemRole;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void Update(string name, string description, int? hierarchyLevel)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name.Trim();
        
        if (description is not null)
            Description = description.Trim();
        
        if (hierarchyLevel.HasValue)
            HierarchyLevel = hierarchyLevel.Value;
        
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (IsSystemRole)
            throw new InvalidOperationException("No se puede desactivar un rol del sistema.");
        
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPermission(RolePermission rolePermission)
    {
        if (!_rolePermissions.Any(rp => rp.PermissionId == rolePermission.PermissionId))
        {
            _rolePermissions.Add(rolePermission);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemovePermission(Guid permissionId)
    {
        var permission = _rolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
        if (permission is not null)
        {
            _rolePermissions.Remove(permission);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
