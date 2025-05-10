using Bookify.Domain.Entities.Bookings;
using Bookify.Infrastructure.Database;
using Bookify.Infrastructure.Repositories.Generic;

namespace Bookify.Infrastructure.Repositories;
internal sealed class BookingRepository(ApplicationDbContext dbContext)
    : Repository<Booking, BookingId>(dbContext), IBookingRepository
{
}
