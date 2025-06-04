using Asp.Versioning;
using Bookify.Api.Constants;
using Bookify.Application.Apartments.Queries;
using Carter;
using MediatR;

namespace Bookify.Api.Endpoints.Apartments;

public class SearchApartmentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();
        
        app.MapGet("/api/v{version:apiVersion}/apartments/search-apartments", async (
                DateOnly startDate,
                DateOnly endDate,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new SearchApartmentsQuery(startDate, endDate);
                var result = await sender.Send(query, cancellationToken);
                return Results.Ok(result.Value);
            })
            .RequireAuthorization() // Uncomment to require auth
            .WithTags(Tags.Apartments) // Optional for OpenAPI grouping
            .WithApiVersionSet(versionSet) // Optional for versioning
            .WithOpenApi() // Optional for OpenAPI documentation
            .WithGroupName("v1") // Optional for OpenAPI grouping
            .WithName("SearchApartmentEndpoint"); // Optional for naming    
    }
}