namespace Bookify.Domain.Entities.Users;

public sealed class RolePermission // Join Entity cho Role và Permission
{
    public RoleId RoleId { get; private set; }
    public PermissionId PermissionId { get; private set; }
    public Role Role { get; private set; }
    public Permission Permission { get; private set; }

    private RolePermission() { } // For EF Core

    public static RolePermission Create(Role role, Permission permission)
    {
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(permission);

        return new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission.Id,
            Role = role,
            Permission = permission
        };
    }
}
