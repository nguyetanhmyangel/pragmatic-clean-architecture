using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Entities.Users;
using Bookify.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;

// Lấy Roles và Permissions từ DB. Kết hợp với cache để tránh truy vấn DB nhiều lần.
internal sealed class AuthorizationService(ApplicationDbContext context, ICacheService cacheService)
{
    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        var cacheKey = $"auth:roles-{identityId}";

        var cachedRoles = await cacheService.GetAsync<UserRolesResponse>(cacheKey);

        if (cachedRoles is not null)        
            return cachedRoles;
        
        var roles = await context.Set<User>()
            .Where(user => user.IdentityId == identityId)
            .Select(user => new UserRolesResponse
            {
                Id = user.Id.Value,
                Roles = user.Roles.ToList()
            })
            .FirstAsync();

        await cacheService.SetAsync(cacheKey, roles);

        return roles;
    }

    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        var cacheKey = $"auth:permissions-{identityId}";

        var cachedPermissions = await cacheService.GetAsync<HashSet<string>>(cacheKey);
        if (cachedPermissions is not null)
            return cachedPermissions;

        var user = await context.Set<User>()
            .Where(u => u.IdentityId == identityId)
            .Include(u => u.Roles)
            .ThenInclude(r => r.Permissions)
            .Include(u => u.UserPermissions)
            .ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync();

        if (user is null)
            return new();

        var permissionsSet = new HashSet<string>();

        foreach (var role in user.Roles)
        {
            foreach (var perm in role.Permissions)
            {
                permissionsSet.Add(perm.Name);
            }
        }

        foreach (var up in user.UserPermissions)
        {
            permissionsSet.Add(up.Permission.Name);
        }

        await cacheService.SetAsync(cacheKey, permissionsSet);

        return permissionsSet;
    }

}
