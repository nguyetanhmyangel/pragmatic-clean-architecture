using Bookify.ShareKernel.Entities;

namespace Bookify.Domain.Entities.Bookings.Events;

public sealed record BookingConfirmedDomainEvent(BookingId BookingId) : IDomainEvent;