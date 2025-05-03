using Bookify.ShareKernel.Event;

namespace Bookify.Domain.Entities.Users.Events;

public sealed record UserCreatedDomainEvent(UserId UserId) : IDomainEvent;