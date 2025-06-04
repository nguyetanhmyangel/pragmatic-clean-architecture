using Ardalis.Specification;
using Bookify.Domain.Entities.Users;
using Bookify.ShareKernel.ValueObjects;

namespace Bookify.Application.Users.Specifications;

public sealed class UserByEmailSpecification : Specification<User>
{
    public UserByEmailSpecification(Email email)
    {
        Query.Where(u => u.Email == email);
        // Nếu cần eagerly load roles
        // Query.AddInclude(u => u.Roles); 
    }
}