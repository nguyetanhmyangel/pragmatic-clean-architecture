using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Bookings.Dtos;
using Bookify.Application.Bookings.Specifications;
using Bookify.Domain.Entities.Bookings;
using Bookify.ShareKernel.Utilities;
using Dapper;

namespace Bookify.Application.Bookings.Queries;
internal sealed class GetBookingByIdQueryHandler : IQueryHandler<GetBookingByIdQuery, BookingResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    //private readonly IUserContext _userContext;

    public GetBookingByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory
        //, IUserContext userContext
        )
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        //_userContext = userContext;
    }

    public async Task<Result<BookingResponse>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();
        var spec = new GetBookingByIdSpecification(request.BookingId);
        var booking = await connection.QueryFirstOrDefaultAsync<BookingResponse>(spec.SqlQuery, spec.Parameters);
        if (booking is null)
        {
            return Result.Failure<BookingResponse>(BookingErrors.NotFound);
        }
        //Resource-based Authorization
        // if (booking is null || booking.UserId != _userContext.UserId)
        //     return Result.Failure<BookingResponse>(BookingErrors.NotFound);

        return booking;
    }
}
