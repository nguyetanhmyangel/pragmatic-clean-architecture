using Bookify.ShareKernel.Entities;

namespace Bookify.Domain.Entities.Bookings.Events;

public sealed record BookingCompletedDomainEvent(BookingId BookingId) : IDomainEvent;