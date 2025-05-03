using Bookify.Application.Abstractions.Caching;
using Bookify.Application.Bookings.Dtos;

namespace Bookify.Application.Bookings.Queries;
public record GetBookingByIdQuery(Guid BookingId) : ICachedQuery<BookingResponse>
{
    public string CacheKey => $"bookings-{BookingId}";
    public TimeSpan? Expiration => null;
}
