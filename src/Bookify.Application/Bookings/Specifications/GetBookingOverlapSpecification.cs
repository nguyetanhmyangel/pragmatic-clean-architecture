
using Ardalis.Specification;
using Bookify.Domain.Entities.Apartments;
using Bookify.Domain.Entities.Bookings;
using Bookify.Domain.Entities.Bookings.Enums;
using Bookify.ShareKernel.Specifications;
using Bookify.ShareKernel.ValueObjects;

namespace Bookify.Application.Bookings.Specifications;
public sealed class GetBookingOverlapSpecification : Specification<Booking>
{
    public GetBookingOverlapSpecification(ApartmentId apartmentId, DateRange duration)
    {
        var activeStatuses = new[]
        {
            BookingStatus.Reserved,
            BookingStatus.Confirmed,
            BookingStatus.Completed
        };

        Query.Where(booking =>
            booking.ApartmentId == apartmentId &&
            booking.Duration.Start == duration.Start &&
            booking.Duration.End == duration.End &&
            activeStatuses.Contains(booking.Status));
    }
}

