using Bookify.ShareKernel.Entities;

namespace Bookify.Domain.Entities.Bookings.Events;

public sealed record BookingCancelledDomainEvent(BookingId BookingId) : IDomainEvent;