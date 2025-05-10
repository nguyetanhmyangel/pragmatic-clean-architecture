using Bookify.ShareKernel.Entities;

namespace Bookify.Domain.Entities.Users.Events;

public sealed record UserCreatedDomainEvent(UserId UserId) : IDomainEvent;