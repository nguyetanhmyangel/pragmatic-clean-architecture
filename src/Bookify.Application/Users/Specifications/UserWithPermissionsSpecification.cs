using Ardalis.Specification;
using Bookify.Domain.Entities.Users;

namespace Bookify.Application.Users.Specifications;

public sealed class UserWithPermissionsSpecification : Specification<User>
{
    public UserWithPermissionsSpecification(string identityId)
    {
        Query
            .Where(u => u.IdentityId == identityId)
            .Include(u => u.Roles)
            .ThenInclude(r => r.Permissions)
            .Include(u => u.UserPermissions)
            .ThenInclude(up => up.Permission);
    }
}
