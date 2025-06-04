using Bookify.ShareKernel.Errors;

namespace Bookify.Domain.Entities.Users;

public static class UserErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        "Users.NotFound",
        $"The user with the Id = '{userId}' was not found");

    public static Error Unauthorized = new(
        "Users.Unauthorized",
        "You are not authorized to perform this action.");
    
    public static readonly Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "The provided credentials were invalid");

    public static readonly Error NotFoundByEmail = new(
        "Users.NotFoundByEmail",
        "The user with the specified email was not found");

    public static readonly Error EmailNotUnique = new(
        "Users.EmailNotUnique",
        "The provided email is not unique");
}