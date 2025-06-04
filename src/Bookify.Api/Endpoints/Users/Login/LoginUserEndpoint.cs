using Asp.Versioning;
using Bookify.Application.Users.Commands.LogInUser;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Bookify.Api.Endpoints.Users.Login;

public class LoginUserEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();
        
        app.MapPost("/api/v{version:apiVersion}/users/login", [AllowAnonymous] async (
            LogInUserRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new LogInUserCommand(
                request.Email,
                request.Password);

            var result = await sender.Send(command, cancellationToken);

            return result.IsFailure
                ? Results.BadRequest(result.Error)
                : Results.Ok(result.Value);
        })
        .AllowAnonymous()
        .WithTags("Users") // Optional for OpenAPI grouping
        .WithName("LoginUserEndpoint")
        .WithGroupName("v1")
        .WithOpenApi()
        .WithApiVersionSet(versionSet)
        .HasApiVersion(1.0)
        ;
    }
}