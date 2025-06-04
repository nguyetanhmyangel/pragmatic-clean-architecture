using Bookify.Domain.Entities.Users.Events;
using Bookify.ShareKernel.Entities;
using Bookify.ShareKernel.ValueObjects;

namespace Bookify.Domain.Entities.Users;

public record UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
}

public sealed class User : Entity<UserId>
{
    private readonly List<Role> _roles = new();
    private readonly List<UserPermission> _userPermissions = new(); // Cho quyền đặc biệt

    private User(UserId id, string firstName, string lastName, Email email)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    private User() : base() { } // For EF Core

    public string IdentityId { get; private set; } = string.Empty;
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();
    public IReadOnlyCollection<UserPermission> UserPermissions => _userPermissions.AsReadOnly(); // Quyền đặc biệt

    public static User Create(string firstName, string lastName, Email email)
    {
        // Thêm validation ở đây nếu cần
        var user = new User(UserId.New(), firstName, lastName, email);
        // user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id)); 

        // Tự động gán role mặc định là "Guest" cho người dùng mới.
        // Role.Guest là một instance tĩnh đã định nghĩa sẵn trong lớp Role.
        // Lưu ý: Điều này chỉ thêm vào collection trong bộ nhớ. EF Core sẽ xử lý việc lưu vào DB.
        user.AddRole(Role.Guest);
        return user;
    }

    public void SetIdentityId(string identityId)
    {
        if (string.IsNullOrWhiteSpace(identityId))
            throw new ArgumentException("IdentityId cannot be empty.", nameof(identityId));
        IdentityId = identityId;
    }

    public void AddRole(Role role)
    {
        if (role is null) throw new ArgumentNullException(nameof(role));
        if (_roles.All(r => r.Id != role.Id))
        {
            _roles.Add(role);
        }
    }

    public void RemoveRole(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);
        var existingRole = _roles.FirstOrDefault(r => r.Id == role.Id);
        if (existingRole != null)
        {
            _roles.Remove(existingRole);
        }
    }

    // Phương thức để thêm quyền đặc biệt trực tiếp cho User
    public void AddDirectPermission(Permission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);
        if (_userPermissions.All(up => up.PermissionId != permission.Id))
        {
            _userPermissions.Add(UserPermission.Create(this, permission));
        }
    }

    public void RemoveDirectPermission(Permission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);
        var userPermissionToRemove = _userPermissions.FirstOrDefault(up => up.PermissionId == permission.Id);
        if (userPermissionToRemove != null)
        {
            _userPermissions.Remove(userPermissionToRemove);
        }
    }

    // kiểm tra quyền (bao gồm cả từ Role và quyền trực tiếp)
    public bool HasPermission(PermissionId permissionId)
    {
        // Kiểm tra quyền trực tiếp
        if (_userPermissions.Any(up => up.PermissionId == permissionId))
        {
            return true;
        }
        // Kiểm tra quyền từ Roles
        return _roles.SelectMany(r => r.Permissions).Any(p => p.Id == permissionId);
    }
}
