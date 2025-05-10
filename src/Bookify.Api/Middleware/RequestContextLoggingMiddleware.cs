using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace Bookify.API.Middleware;

/// <summary>
/// Middleware to give the flexibility of either taking in the correlation id externally from another service talking
/// with my API allowing me to trace a single request across multiple services in a microservice environment
/// </summary>
public class RequestContextLoggingMiddleware(RequestDelegate next)
{
    /*
        RequestContextLoggingMiddleware đính kèm CorrelationId vào ngữ cảnh log cho từng HTTP request.
        Mục tiêu chính là giúp theo dõi và gỡ lỗi các request trong hệ thống phân tán
        bằng cách gắn mã định danh duy nhất cho mỗi request.
    */

    //Lấy Correlation ID từ header
    private const string CorrelationIdHeaderName = "Correlation-Id";

    public Task Invoke(HttpContext context)
    {
        //Đẩy CorrelationId vào LogContext của Serilog
        using (LogContext.PushProperty("CorrelationId", GetCorrelationId(context)))
        {
            return next.Invoke(context);
        }
    }

    private static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(
            CorrelationIdHeaderName,
            out StringValues correlationId);

        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}
