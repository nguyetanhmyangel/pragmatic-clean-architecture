﻿using Bookify.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Bookify.Infrastructure.Authorization;

internal sealed class CustomClaimsTransformation(IServiceProvider serviceProvider) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Nếu đã có Role và Sub claims → return.
        if (principal.HasClaim(claim =>  claim.Type == ClaimTypes.Role) &&
            principal.HasClaim(claim => claim.Type == JwtRegisteredClaimNames.Sub))
        {
            return principal;
        }
        
        // Nếu ko có, Thêm claim Sub và ClaimTypes.Role vào ClaimsPrincipal.
        using var scope = serviceProvider.CreateScope();

        var authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();

        var identityId = principal.GetIdentityId();

        var userRoles = await authorizationService.GetRolesForUserAsync(identityId);

        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, userRoles.Id.ToString()));

        foreach (var role in userRoles.Roles)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
        }

        principal.AddIdentity(claimsIdentity);

        return principal;
    }
}
