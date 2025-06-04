using Bookify.Domain.Entities.Users;
using Bookify.ShareKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(userId => userId.Value, value => new UserId(value));

        builder.Property(u => u.FirstName).HasMaxLength(200).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(200).IsRequired();
        builder.Property(u => u.IdentityId).HasMaxLength(128).IsRequired(false); // Có thể null ban đầu
        builder.HasIndex(u => u.IdentityId).IsUnique().HasFilter("[IdentityId] IS NOT NULL");


        builder.Property(u => u.Email)
            .HasMaxLength(254)
            .IsRequired()
            .HasConversion(
                email => email.Value,
                value => Email.Create(value)); // Sử dụng Email.Create
        builder.HasIndex(u => u.Email).IsUnique();


        // Mối quan hệ nhiều-nhiều với Roles đã được cấu hình trong RoleConfiguration
        // builder.HasMany(user => user.Roles)
        //     .WithMany(role => role.Users)
        //     .UsingEntity(j => j.ToTable("user_roles")); // Đảm bảo tên bảng join nhất quán

        // Mối quan hệ một-nhiều với UserPermission (cho quyền đặc biệt)
        builder.HasMany(user => user.UserPermissions)
            .WithOne(up => up.User) // UserPermission có một User
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Nếu User bị xóa, UserPermission liên quan cũng bị xóa
    }
}

