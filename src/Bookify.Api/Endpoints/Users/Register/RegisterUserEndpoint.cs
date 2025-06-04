using Asp.Versioning;
using Bookify.Application.Users.Commands.RegisterUser;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Bookify.Api.Endpoints.Users.Register;

public class RegisterUserEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();
        
        app.MapPost("/api/v{version:apiVersion}/users/register", [AllowAnonymous] async (
            RegisterUserRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password);

            var result = await sender.Send(command, cancellationToken);

            return result.IsFailure
                ? Results.BadRequest(result.Error)
                : Results.Ok(result.Value);
        })
        .RequireAuthorization() // Uncomment to require auth
        .WithTags("Users") // Optional for OpenAPI grouping
        .WithName("RegisterUserEndpoint")
        .WithOpenApi()
        .WithGroupName("v1")
        .WithApiVersionSet(versionSet)
        .HasApiVersion(1.0)
        ;
    }
}