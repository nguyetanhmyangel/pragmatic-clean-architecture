using Bookify.Domain.Entities.Users;
using Bookify.Infrastructure.Database;
using Bookify.Infrastructure.Repositories.Generic;

namespace Bookify.Infrastructure.Repositories;
internal sealed class UserRepository(ApplicationDbContext dbContext)
    : Repository<User, UserId>(dbContext), IUserRepository
{
    // private readonly ApplicationDbContext _dbContext;
    //
    // public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    // {
    //     _dbContext = dbContext;
    // }
    
    public override void Add(User user)
    {
        //This will tell EF Core that any roles present on our user object are already inside of the database, and you don't
        //need to insert them again 
        //Attach là một thao tác đồng bộ vì nó chỉ cập nhật trạng thái của thực thể trong change tracker, không liên quan đến I/O. Do đó, không cần (và EF Core cũng không cung cấp) phiên bản bất đồng bộ của Attach
        foreach (var role in user.Roles)
        {
            DbContext.Attach(role);
        }

        DbContext.Add(user);
    }
}
