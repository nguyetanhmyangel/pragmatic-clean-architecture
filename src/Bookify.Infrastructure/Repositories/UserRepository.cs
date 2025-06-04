using Bookify.Application.Users.Specifications;
using Bookify.Domain.Entities.Users;
using Bookify.Infrastructure.Database;
using Bookify.Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories;
internal sealed class UserRepository(ApplicationDbContext dbContext)
    : GenericRepository<User, UserId>(dbContext), IUserRepository
{
    public async Task<User?> GetByIdentityIdAsync(string identityId, CancellationToken cancellationToken = default)
    {
        var spec = new UserWithPermissionsSpecification(identityId);
        return await GetSingleAsync(spec, cancellationToken);
    }
    
    public new void Add(User user)
    {
        //This will tell EF Core that any roles present on our user object are already inside of the database, and you don't
        //need to insert them again 
        //Attach là một thao tác đồng bộ vì nó chỉ cập nhật trạng thái của thực thể trong change tracker, không liên quan đến I/O. Do đó, không cần (và EF Core cũng không cung cấp) phiên bản bất đồng bộ của Attach
        
        foreach (var role in user.Roles)
        {
            if (dbContext.Entry(role).State == EntityState.Detached) // Chỉ attach nếu role bị detached
            {
                dbContext.Attach(role); // Đánh dấu roles là tracked, quan trọng cho static roles như Role.Guest
            }
        }
    }
}
