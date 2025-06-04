using Bookify.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;
/// <summary>
/// Dùng .HasMany(...).WithMany(...) mà không dùng .UsingEntity<>(),
/// thì EF Core sẽ sinh tự động bảng trung gian như role_user ,trung gian giữa roles và users
/// </summary>
internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // builder.HasMany(role => role.Permissions)
        //     .WithMany()
        //     .UsingEntity<RolePermission>();
        // builder.HasData(Role.Guest);
        builder.ToTable("roles");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasConversion(roleId => roleId.Value, value => new RoleId(value));
        builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(r => r.Name).IsUnique();

        // Mối quan hệ nhiều-nhiều giữa Role và User (thông qua bảng join user_roles)
        builder.HasMany(role => role.Users)
            .WithMany(user => user.Roles)
            .UsingEntity(j => j.ToTable("user_roles")); // EF Core tự tạo bảng join

        // Mối quan hệ nhiều-nhiều giữa Role và Permission (thông qua RolePermission)
        builder.HasMany(role => role.Permissions)
            .WithMany() // Permission không cần navigation property ngược lại Roles
            .UsingEntity<RolePermission>(
                "role_permissions", // Tên bảng join
                l => 
                    l.HasOne<Permission>().WithMany().HasForeignKey(rp => rp.PermissionId),
                r => 
                    r.HasOne<Role>().WithMany().HasForeignKey(rp => rp.RoleId) 
                // Không cần WithMany(role => role.RolePermissions) vì Role không có collection đó
            );


        builder.HasData(Role.Guest);
    }
}
