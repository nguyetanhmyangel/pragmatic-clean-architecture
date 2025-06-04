using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Users.Commands.LogInUser;
using Bookify.Application.Users.Dtos;
using Bookify.Domain.Entities.Users;
using Bookify.ShareKernel.Utilities;

namespace Bookify.Application.Users.Commands;
internal sealed class LogInUserCommandHandler(IJwtService jwtService)
    : ICommandHandler<LogInUserCommand, AccessTokenResponse>
{
    public async Task<Result<AccessTokenResponse>> Handle(LogInUserCommand request, CancellationToken cancellationToken)
    {
        var result = await jwtService.GetAccessTokenAsync(
            request.Email,
            request.Password, 
            cancellationToken);
        if (result.IsFailure) return Result.Failure<AccessTokenResponse>(UserErrors.InvalidCredentials);
        return new AccessTokenResponse(result.Value);
    }
}
