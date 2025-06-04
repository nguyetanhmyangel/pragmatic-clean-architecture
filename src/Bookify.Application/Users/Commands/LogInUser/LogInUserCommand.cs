using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Users.Dtos;

namespace Bookify.Application.Users.Commands.LogInUser;
public sealed record LogInUserCommand(string Email, string Password) : ICommand<AccessTokenResponse>;