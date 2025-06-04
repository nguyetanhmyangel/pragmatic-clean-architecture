using Microsoft.AspNetCore.Authorization;

namespace Bookify.Infrastructure.Authorization;

// dùng [HasPermission("Permission.ViewContract")] Thay vì [Authorize("Permission.ViewContract")]
public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission);
