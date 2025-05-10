using Bookify.ShareKernel.Repositories;

namespace Bookify.Domain.Entities.Users;

public interface IUserRepository: IRepository<User, UserId>
{
    new void Add(User user);
}