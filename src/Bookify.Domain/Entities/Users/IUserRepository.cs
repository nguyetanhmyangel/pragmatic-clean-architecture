using Bookify.ShareKernel.BaseRepository;

namespace Bookify.Domain.Entities.Users;

public interface IUserRepository: IRepository<User, UserId>
{
    new void Add(User user);
}