namespace Bookify.Domain.Entities.Users;

// Join Entity cho User và Permission (quyền đặc biệt)
public class UserPermission
{
    public UserId UserId { get; private set; }
    public PermissionId PermissionId { get; private set; }
    public User User { get; private set; }
    public Permission Permission { get; private set; }

    private UserPermission() { } // For EF Core

    public static UserPermission Create(User user, Permission permission)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(permission);

        return new UserPermission
        {
            UserId = user.Id,
            PermissionId = permission.Id,
            User = user,
            Permission = permission
        };
    }
}
