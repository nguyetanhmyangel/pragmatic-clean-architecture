using Bookify.API.Middleware;
using Bookify.Infrastructure;
using Bookify.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bookify.API.Extensions;

public static class ApplicationBuilderExtension
{
    public static IApplicationBuilder UseRequestContextLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestContextLoggingMiddleware>();
        return app;
    }
}
