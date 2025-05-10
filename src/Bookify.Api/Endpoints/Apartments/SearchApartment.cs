using Bookify.Application.Apartments.Queries;
using Carter;
using MediatR;

namespace Bookify.Api.Endpoints.Apartments;

public class SearchApartmentModule() : CarterModule("api/apartments")
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
                DateOnly startDate,
                DateOnly endDate,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new SearchApartmentsQuery(startDate, endDate);
                var result = await sender.Send(query, cancellationToken);
                return Results.Ok(result.Value);
            })
            //.RequireAuthorization() // Uncomment to require auth
            .WithTags("Apartments") // Optional for OpenAPI grouping
            .WithName("SearchApartments");
    }
}