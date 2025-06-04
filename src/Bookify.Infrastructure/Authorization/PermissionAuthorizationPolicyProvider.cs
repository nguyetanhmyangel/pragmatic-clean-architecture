using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authorization;

// Cho phép tạo chính sách phân quyền động (dynamic authorization policy).
// Ví dụ: khi ghi [Authorize("Permission.ViewContract")], chính sách "Permission.ViewContract" sẽ tự động được tạo nếu chưa có.
internal sealed class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    private readonly AuthorizationOptions _authorizationOptions = options.Value;

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        //base.GetPolicyAsync(policyName) gọi đến DefaultAuthorizationPolicyProvider
        // để xem chính sách (policy) với tên policyName có được định nghĩa sẵn trong
        // AuthorizationOptions không.Nếu có rồi thì trả về chính sách đó
        var policy = await base.GetPolicyAsync(policyName);
        if (policy is not null)
            return policy;
        
        // Nếu chưa có, Tạo một chính sách mới bằng cách sử dụng AuthorizationPolicyBuilder.
        // Thêm một PermissionRequirement chứa policyName.
        // Gọi _authorizationOptions.AddPolicy để đăng ký chính sách mới vào bộ nhớ (không phải database).
        var permissionPolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();
        _authorizationOptions.AddPolicy(policyName, permissionPolicy);
        return permissionPolicy;
    }
}
