using Bookify.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrastructure.Authorization;

// kiểm tra xem người dùng hiện tại có quyền (permission) tương ứng với PermissionRequirement hay không.
internal sealed class PermissionAuthorizationHandler(IServiceProvider serviceProvider)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (!context.User.Identity!.IsAuthenticated)
            return;

        using var scope = serviceProvider.CreateScope();
        var authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();
        // trích xuất sub hoặc một claim duy nhất từ token của Keycloak
        // (ví dụ: "sub" hoặc "preferred_username"), dùng để xác định người dùng
        var identityId = context.User.GetIdentityId();

        //TODO: Introduce caching to avoid calling the database every time
        HashSet<string> permissions = await authorizationService.GetPermissionsForUserAsync(identityId);
        // Nếu quyền được yêu cầu tồn tại trong danh sách, thì gọi context.Succeed()
        if (permissions.Contains(requirement.Permissions))
            context.Succeed(requirement);
    }
}
