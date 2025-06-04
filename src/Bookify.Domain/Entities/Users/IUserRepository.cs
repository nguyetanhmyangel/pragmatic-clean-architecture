using Bookify.ShareKernel.Repositories;

namespace Bookify.Domain.Entities.Users;

public interface IUserRepository: IRepository<User, UserId>
{
    //public Task<User?> GetByEmailAsync(string email);
    new void Add(User user);
}