using Application.IService;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;

namespace Presentation.MiddleWares
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache cache;

        public TokenMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            _next = next;
            this.cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated == true)
            {
                string authHeader = context.Request.Headers["Authorization"];
                var userEmail = context.User.FindFirst(ClaimTypes.Email).Value;
                string token = authHeader.Substring("Bearer ".Length).Trim();
                if (context.Request.Path.Value.StartsWith("/api/refresh") || context.Request.Path.Value.StartsWith("/api/login")) await _next(context);
                string source = context.Request.Headers["User-Agent"];
                string key = $"{userEmail}-{source}";
                //string? userToken = null;
                string? cachedToken = await cache.GetStringAsync(key );
                if (cachedToken is null || !cachedToken.Equals(token))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync("Error :  Invalid Token");
                    return ;
                }

            }
            await _next(context);
        }
    }

    public static class TokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenMiddleware>();
        }
    }
}
