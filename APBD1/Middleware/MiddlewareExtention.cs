using Microsoft.AspNetCore.Builder;

namespace APBD1.Middleware
{
    public static class MiddlewareExtention
    {
        public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}