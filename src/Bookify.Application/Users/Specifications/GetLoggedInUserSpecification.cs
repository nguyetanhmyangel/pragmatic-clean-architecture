using Bookify.Application.Users.Dtos;
using Bookify.ShareKernel.Specifications;

namespace Bookify.Application.Users.Specifications;

public class GetLoggedInUserSpecification(string identityId): ISqlSpecification<UserResponse>
{
    public string SqlQuery => """
                              SELECT
                                  id AS Id,
                                  first_name AS FirstName,
                                  last_name AS LastName,
                                  email AS Services
                              FROM users
                              WHERE identity_id = @IdentityId
                              """;
    public object Parameters => new { IdentityId = identityId };
}