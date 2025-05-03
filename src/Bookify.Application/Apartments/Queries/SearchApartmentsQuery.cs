using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Apartments.Dtos;

namespace Bookify.Application.Apartments.Queries;

public record SearchApartmentsQuery(DateOnly StartDate, DateOnly EndDate) : IQuery<IReadOnlyList<ApartmentResponse>>;