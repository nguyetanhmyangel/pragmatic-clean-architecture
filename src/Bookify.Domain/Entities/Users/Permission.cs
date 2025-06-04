using Bookify.ShareKernel.Entities;

namespace Bookify.Domain.Entities.Users;

public record PermissionId(Guid Value)
{
    public static PermissionId New() => new(Guid.NewGuid());
}

public sealed class Permission : Entity<PermissionId>
{
    private Permission() : base() { } // For EF Core
    private Permission(PermissionId id, string name) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be empty.", nameof(name));
        Name = name;
    }
    public string Name { get; private set; }

    public static Permission Create(string name) // Thêm factory method nếu cần tạo Permission động
    {
        return new Permission(PermissionId.New(), name);
    }
    public static readonly Permission UsersRead = 
        new(new PermissionId(new Guid("fedcba09-8765-4321-0fed-cba987654321")), 
            "users:read");
}
