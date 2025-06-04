using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Extensions;

public static class ApiEndpointExtensions
{
    public static RouteGroupBuilder MapVersionedGroup(
        this IEndpointRouteBuilder builder,
        string pattern,
        IEnumerable<ApiVersion> versions,
        //Func<AuthorizationBuilder, AuthorizationBuilder>? authConfig = null,
        bool allowAnonymous = false,
        string? swaggerGroup = null)
    {
        var versionSet = builder.NewApiVersionSet().HasApiVersion(versions.First());

        foreach (var version in versions.Skip(1))
        {
            versionSet = versionSet.HasApiVersion(version);
        }

        var set = versionSet.ReportApiVersions().Build();

        var group = builder.MapGroup(pattern).WithApiVersionSet(set);

        foreach (var version in versions)
        {
            group = group.MapToApiVersion(version);
        }

        if (!string.IsNullOrWhiteSpace(swaggerGroup))
        {
            group.WithGroupName(swaggerGroup);
        }

        if (allowAnonymous)
        {
            group.AllowAnonymous();
        }
        // else if (authConfig != null)
        // {
        //     var builderAuth = new AuthorizationBuilder(group);
        //     authConfig(builderAuth);
        // }

        return group;
    }

    // public class AuthorizationBuilder(RouteGroupBuilder group)
    // {
    //     public AuthorizationBuilder RequireRoles(params string[] roles)
    //     {
    //         group.RequireAuthorization(policy => policy.RequireRole(roles));
    //         return this;
    //     }
    //
    //     public AuthorizationBuilder RequirePermissions(params string[] permissions)
    //     {
    //         group.RequireAuthorization(policy =>
    //         {
    //             policy.RequireClaim("Permission", permissions);
    //         });
    //         return this;
    //     }
    //
    //     public AuthorizationBuilder RequireAnyPermission(params string[] permissions)
    //     {
    //         group.RequireAuthorization(policy =>
    //         {
    //             policy.RequireAssertion(context =>
    //             {
    //                 return context.User.Claims
    //                     .Where(c => c.Type == "Permission")
    //                     .Any(c => permissions.Contains(c.Value));
    //             });
    //         });
    //         return this;
    //     }
    //
    //     public AuthorizationBuilder RequireClaims(string claimType, params string[] claimValues)
    //     {
    //         group.RequireAuthorization(policy =>
    //         {
    //             policy.RequireClaim(claimType, claimValues);
    //         });
    //         return this;
    //     }
    //
    //     public AuthorizationBuilder RequireAnyClaim(string claimType, params string[] claimValues)
    //     {
    //         group.RequireAuthorization(policy =>
    //         {
    //             policy.RequireAssertion(context =>
    //             {
    //                 return context.User.Claims
    //                     .Where(c => c.Type == claimType)
    //                     .Any(c => claimValues.Contains(c.Value));
    //             });
    //         });
    //         return this;
    //     }
    //
    //     public AuthorizationBuilder RequireRoleOrPermission(string[] roles, string[] permissions)
    //     {
    //         group.RequireAuthorization(policy =>
    //         {
    //             policy.RequireAssertion(context =>
    //             {
    //                 var user = context.User;
    //                 return roles.Any(role => user.IsInRole(role)) ||
    //                        user.Claims
    //                             .Where(c => c.Type == "Permission")
    //                             .Any(c => permissions.Contains(c.Value));
    //             });
    //         });
    //         return this;
    //     }
    // }
}