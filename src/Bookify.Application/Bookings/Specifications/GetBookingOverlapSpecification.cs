
using System.Linq.Expressions;
using Bookify.Domain.Entities.Apartments;
using Bookify.Domain.Entities.Bookings;
using Bookify.Domain.Entities.Bookings.Enums;
using Bookify.ShareKernel.Specifications;
using Bookify.ShareKernel.ValueObjects;

namespace Bookify.Application.Bookings.Specifications;
public sealed class GetBookingOverlapSpecification : ISpecification<Booking>
{
    public Expression<Func<Booking, bool>> Criteria { get; }
    public List<Expression<Func<Booking, object>>> Includes => new();
    public Func<IQueryable<Booking>, IOrderedQueryable<Booking>>? OrderBy => null;
    public Func<IQueryable<Booking>, IOrderedQueryable<Booking>>? OrderByDescending => null;

    public GetBookingOverlapSpecification(ApartmentId apartmentId, DateRange duration)
    {
        var activeStatuses = new[]
        {
            BookingStatus.Reserved,
            BookingStatus.Confirmed,
            BookingStatus.Completed
        };

        Criteria = booking =>
            booking.ApartmentId == apartmentId &&
            booking.Duration.Start == duration.Start &&
            booking.Duration.End == duration.End &&
            activeStatuses.Contains(booking.Status);
    }
}
