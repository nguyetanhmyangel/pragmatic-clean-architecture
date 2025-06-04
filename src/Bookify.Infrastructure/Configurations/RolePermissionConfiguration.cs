using Bookify.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;
public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions"); // Đã được định nghĩa trong RoleConfiguration.UsingEntity
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // Cấu hình các khóa ngoại đã được thực hiện trong RoleConfiguration.UsingEntity
        // Không cần cấu hình lại HasOne().WithMany() ở đây nếu đã dùng UsingEntity 

        // Seed data cho RolePermission
        builder.HasData(
            RolePermission.Create(Role.Guest, Permission.UsersRead)
        );
    }
}
