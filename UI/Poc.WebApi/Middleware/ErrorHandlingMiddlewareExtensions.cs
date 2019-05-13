using Microsoft.AspNetCore.Builder;

namespace Poc.WebApi.Middleware
{
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UseErrorHandlingMiddleware>();
        }
    }
}
