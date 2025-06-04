using Bookify.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable("user_permissions");
        builder.HasKey(up => new { up.UserId, up.PermissionId }); // Khóa chính phức hợp

        // Mối quan hệ từ UserPermission đến User (đã được cấu hình trong UserConfiguration)
        builder.HasOne(up => up.User)
            .WithMany(u => u.UserPermissions) // User có nhiều UserPermission
            .HasForeignKey(up => up.UserId);

        // Mối quan hệ từ UserPermission đến Permission
        builder.HasOne(up => up.Permission)
            .WithMany() // Permission không cần navigation property ngược lại UserPermissions (trừ khi bạn muốn)
            .HasForeignKey(up => up.PermissionId);
    }
}