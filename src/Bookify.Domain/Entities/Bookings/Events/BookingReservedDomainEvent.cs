using Bookify.ShareKernel.Event;

namespace Bookify.Domain.Entities.Bookings.Events;

public sealed record BookingReservedDomainEvent(BookingId BookingId) : IDomainEvent;