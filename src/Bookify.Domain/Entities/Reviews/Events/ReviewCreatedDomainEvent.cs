using Bookify.ShareKernel.Entities;

namespace Bookify.Domain.Entities.Reviews.Events;

public sealed record ReviewCreatedDomainEvent(ReviewId ReviewId) : IDomainEvent;