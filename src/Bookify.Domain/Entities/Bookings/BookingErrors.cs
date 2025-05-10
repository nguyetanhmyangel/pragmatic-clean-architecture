using Bookify.ShareKernel.Errors;

namespace Bookify.Domain.Entities.Bookings;

public static class BookingErrors
{
    public static Error NotFound = new(
        "Booking.NotFound",
        $"The booking with the was not found");

    public static Error Conflict(Guid bookingId) => Error.Conflict(
        "Booking.Overlap",
        $"The current booking with the Id ={bookingId} is Conflict with an existing one");

    public static readonly Error NotReserved = new(
        "Booking.NotReserved",
        $"The booking is not pending");

    public static readonly Error NotConfirmed = new(
        "Booking.NotConfirmed",
        "The booking is not confirmed");

    public static Error AlreadyStarted = new(
        "Booking.AlreadyStarted",
        $"The booking has already started");
}