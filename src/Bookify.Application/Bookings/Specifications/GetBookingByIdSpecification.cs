using Bookify.Application.Bookings.Dtos;
using Bookify.ShareKernel.Specifications;

namespace Bookify.Application.Bookings.Specifications;

public class GetBookingByIdSpecification(Guid bookingId) : ISqlSpecification<BookingResponse>
{
    // private readonly Guid _bookingId;
    //
    // public GetBookingByIdSpecification(Guid bookingId)
    // {
    //     _bookingId = bookingId;
    // }
    public string SqlQuery => """
                              SELECT
                                              id AS Id,
                                              apartment_id AS ApartmentId,
                                              user_id AS UserId,
                                              status AS Status,
                                              price_for_period_amount AS PriceAmount,
                                              price_for_period_currency AS PriceCurrency,
                                              cleaning_fee_amount AS CleaningFeeAmount,
                                              cleaning_fee_currency AS CleaningFeeCurrency,
                                              amenities_up_charge_amount AS AmenitiesUpChargeAmount,
                                              amenities_up_charge_currency AS AmenitiesUpChargeCurrency,
                                              total_price_amount AS TotalPriceAmount,
                                              total_price_currency AS TotalPriceCurrency,
                                              duration_start AS DurationStart,
                                              duration_end AS DurationEnd,
                                              created_on_utc AS CreatedOnUtc
                                          FROM bookings
                                          WHERE id = @BookingId
                              """;
    public object Parameters => new { BookingId = bookingId };
}