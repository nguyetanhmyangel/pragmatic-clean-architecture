using Bookify.ShareKernel.Event;

namespace Bookify.Domain.Entities.Bookings.Events;

public sealed record BookingConfirmedDomainEvent(BookingId BookingId) : IDomainEvent;