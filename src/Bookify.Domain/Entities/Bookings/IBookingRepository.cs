using Bookify.Domain.Entities.Apartments;
using Bookify.ShareKernel.Repositories;
using Bookify.ShareKernel.ValueObjects;

namespace Bookify.Domain.Entities.Bookings;

public interface IBookingRepository: IRepository<Booking, BookingId>
{
    // Task<Booking> GetByIdAsync(BookingId id, CancellationToken cancellationToken = default);
    //
    // Task<bool> IsOverlappingAsync(
    //     Apartment apartment,
    //     DateRange duration,
    //     CancellationToken cancellationToken = default);
    //
    // void Add(Booking booking);
}