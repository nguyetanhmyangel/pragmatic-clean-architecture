using Bookify.Application.Apartments.Dtos;
using Bookify.Domain.Entities.Bookings.Enums;
using Bookify.ShareKernel.Specification;

namespace Bookify.Application.Apartments.Specifications;

public sealed class SearchApartmentsSpecification: ISqlSpecification<ApartmentResponse>
{
    private static readonly int[] ActiveBookingStatuses =
    [
        (int)BookingStatus.Reserved,
        (int)BookingStatus.Confirmed,
        (int)BookingStatus.Completed
    ];

    public SearchApartmentsSpecification(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be before or equal to end date.");

        Parameters = new
        {
            StartDate = startDate,
            EndDate = endDate,
            ActiveBookingStatuses
        };
    }

    public string SqlQuery => """
                                 SELECT
                                     a.id AS Id,
                                     a.name AS Name,
                                     a.description AS Description,
                                     a.price_amount AS Price,
                                     a.price_currency AS Currency,
                                     a.address_country AS Country,
                                     a.address_state AS State,
                                     a.address_zip_code AS ZipCode,
                                     a.address_city AS City,
                                     a.address_street AS Street
                                 FROM apartments a
                                 WHERE NOT EXISTS
                                 (
                                     SELECT 1
                                     FROM bookings b
                                     WHERE
                                         b.apartment_id = a.id AND
                                         b.duration_start <= @EndDate AND
                                         b.duration_end >= @StartDate AND
                                         b.status = ANY(@ActiveBookingStatuses)
                                 )
                                 """;

    public object Parameters { get; }
}