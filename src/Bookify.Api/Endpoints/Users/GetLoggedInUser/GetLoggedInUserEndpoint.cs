using Asp.Versioning;
using Bookify.Application.Users.Queries;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Endpoints.Users.GetLoggedInUser;

public class GetLoggedInUserEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .HasApiVersion(new ApiVersion(2, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/users")
            .WithApiVersionSet(apiVersionSet)
            .RequireAuthorization() // Áp dụng xác thực cho toàn bộ nhóm
            .WithTags("Users");

        // Endpoint cho v1
        group.MapGet("/current-user", async (
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new GetLoggedInUserQuery();
                var result = await sender.Send(query, cancellationToken);
                return result.IsFailure
                    ? Results.Problem(detail: result.Error.Description, statusCode: 400)
                    : Results.Ok(result.Value);
            })
            .HasApiVersion(new ApiVersion(1, 0))
            .WithOpenApi()
            .WithGroupName("v1")
            //.RequireAuthorization(Permissions.UsersRead)
            .WithName("GetLoggedInUserV1");

        // Endpoint cho v2
        group.MapGet("/current-user", async (
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new GetLoggedInUserQuery();
                var result = await sender.Send(query, cancellationToken);
                return result.IsFailure
                    ? Results.Problem(detail: result.Error.Description, statusCode: 400)
                    : Results.Ok(result.Value);
            })
            .HasApiVersion(new ApiVersion(1, 0))
            .WithOpenApi()
            .WithGroupName("v2")
            //.RequireAuthorization(Permissions.UsersRead)
            .WithName("GetLoggedInUserV2");
    }
}