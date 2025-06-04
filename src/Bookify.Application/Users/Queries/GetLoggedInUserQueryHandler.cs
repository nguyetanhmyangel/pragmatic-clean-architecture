using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Users.Dtos;
using Bookify.Application.Users.Specifications;
using Bookify.ShareKernel.Utilities;
using Dapper;

namespace Bookify.Application.Users.Queries;
internal sealed class GetLoggedInUserQueryHandler(ISqlConnectionFactory sqlConnectionFactory, IUserContext userContext)
    : IQueryHandler<GetLoggedInUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetLoggedInUserQuery request, CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();
        // Tạo specification với IdentityId từ userContext
        var specification = new GetLoggedInUserSpecification(userContext.IdentityId);
        // Thực thi truy vấn sử dụng specification
        return await connection.QuerySingleOrDefaultAsync<UserResponse>(
            specification.SqlQuery,
            specification.Parameters);     
    }
}
