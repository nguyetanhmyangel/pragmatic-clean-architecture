namespace Bookify.Api.Endpoints.Users.Register;

public sealed record RegisterUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string Password);
