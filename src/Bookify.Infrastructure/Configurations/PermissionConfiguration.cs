using Bookify.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;
public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(permissionId => permissionId.Value, value => new PermissionId(value));
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(p => p.Name).IsUnique();
        //Seed initial data
        builder.HasData(
            Permission.UsersRead
            // Thêm các quyền tĩnh khác ở đây nếu cần seed
        );
    }
}
