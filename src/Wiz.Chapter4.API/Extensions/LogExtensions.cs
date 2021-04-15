using Microsoft.AspNetCore.Builder;
using Wiz.Chapter4.API.Middlewares;

namespace Wiz.Chapter4.API.Extensions
{
    public static class LogExtensions
    {
        public static IApplicationBuilder UseLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogMiddleware>();
        }
    }
}
