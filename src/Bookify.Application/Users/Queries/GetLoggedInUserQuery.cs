using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Users.Dtos;

namespace Bookify.Application.Users.Queries;
public sealed record GetLoggedInUserQuery : IQuery<UserResponse>;
