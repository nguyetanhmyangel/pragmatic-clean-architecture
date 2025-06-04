using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Users.Specifications;
using Bookify.Domain.Entities.Users;
using Bookify.ShareKernel.Errors;
using Bookify.ShareKernel.Repositories;
using Bookify.ShareKernel.Utilities;
using Bookify.ShareKernel.ValueObjects;

namespace Bookify.Application.Users.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IAuthenticationService authenticationService,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check if already registered in App DB
        //var existingUser = await userRepository.GetByEmailAsync(request.Services);
        var email = Email.Create(request.Email);
        var spec = new UserByEmailSpecification(email);
        var existingUser = await userRepository.GetSingleAsync(spec, cancellationToken);
        if (existingUser is not null)
            return Result.Failure<Guid>(new Error("User.AlreadyExists", "User already exists in application."));
        // Create domain user
        var user = User.Create(request.FirstName, request.LastName, email);
        // register user in keycloak
        var registerResult = await authenticationService.RegisterAsync(
            user,
            request.Password,
            cancellationToken);
        if (registerResult.IsFailure)
        {
            return Result.Failure<Guid>(registerResult.Error);
        }
        user.SetIdentityId(registerResult.Value);
        // add user to app db
        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        //return user.Id.Value;
        return Result.Success(user.Id.Value);
    }
}
