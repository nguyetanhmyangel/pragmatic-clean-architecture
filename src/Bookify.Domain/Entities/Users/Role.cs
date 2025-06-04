using Bookify.ShareKernel.Entities;

namespace Bookify.Domain.Entities.Users;

public record RoleId(Guid Value)
{
    public static RoleId New() => new(Guid.NewGuid());
}

public sealed class Role : Entity<RoleId>
{
    public static readonly Role Guest = 
        new(new RoleId(new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef")), 
            "Guest");
    
    private readonly List<Permission> _permissions = new();

    // For EF Core and controlled instantiation
    private Role() : base() { }
    public Role(RoleId id, string name) : base(id)
    {
        Name = name;
    }

    public string Name { get; private set; }
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();
    public ICollection<User> Users { get; private set; } = new HashSet<User>(); // EF Core quản lý

    public static Role Create(string name)
    {
        // Có thể thêm validation cho 'name' ở đây
        return new Role(RoleId.New(), name);
    }

    public void AddPermission(Permission permission)
    {
        if (permission is null) throw new ArgumentNullException(nameof(permission));
        if (_permissions.All(p => p.Id != permission.Id)) // Kiểm tra bằng Id để đảm bảo tính duy nhất
        {
            _permissions.Add(permission);
        }
    }

    public void RemovePermission(Permission permission)
    {
        if (permission is null) throw new ArgumentNullException(nameof(permission));
        var existingPermission = _permissions.FirstOrDefault(p => p.Id == permission.Id);
        if (existingPermission != null)
        {
            _permissions.Remove(existingPermission);
        }
    }
}
