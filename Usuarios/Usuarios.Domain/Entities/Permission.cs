namespace Usuarios.Domain.Entities;

/// <summary>
/// Representa un permiso granular en el sistema RBAC.
/// Los permisos definen acciones específicas que pueden realizarse en recursos.
/// </summary>
public class Permission
{
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Código único del permiso (e.g., "users.create", "catalog.read")
    /// </summary>
    public string Code { get; private set; } = string.Empty;
    
    /// <summary>
    /// Nombre descriptivo del permiso
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// Descripción detallada del permiso
    /// </summary>
    public string Description { get; private set; } = string.Empty;
    
    /// <summary>
    /// Módulo al que pertenece el permiso (e.g., "users", "catalog", "orders")
    /// </summary>
    public string Module { get; private set; } = string.Empty;
    
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    private Permission()
    {
    }

    public Permission(string code, string name, string description, string module)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("El código del permiso es obligatorio.", nameof(code));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del permiso es obligatorio.", nameof(name));

        Id = Guid.NewGuid();
        Code = code.Trim().ToLowerInvariant();
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Module = module?.Trim().ToLowerInvariant() ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string description)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name.Trim();
        
        if (description is not null)
            Description = description.Trim();
    }
}
