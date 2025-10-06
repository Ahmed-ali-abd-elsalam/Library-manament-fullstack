using Application.IService;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;

namespace Presentation.MiddleWares
{
    public class TokenMiddleware :IMiddleware
    {
        private readonly IDistributedCache cache;

        public TokenMiddleware( IDistributedCache cache)
        {
            this.cache = cache;
        }


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.User.Identity.IsAuthenticated == true)
            {
                string authHeader = context.Request.Headers["Authorization"];
                var userEmail = context.User.FindFirst(ClaimTypes.Email).Value;
                string token = authHeader.Substring("Bearer ".Length).Trim();
                if (context.Request.Path.Value.StartsWith("/api/refresh") || context.Request.Path.Value.StartsWith("/api/login")) await next(context);
                string source = context.Request.Headers["User-Agent"];
                string key = $"{userEmail}-{source}";
                //string? userToken = null;
                string? cachedToken = await cache.GetStringAsync(key);
                if (cachedToken is null || !cachedToken.Equals(token))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync("Error :  Invalid Token");
                    return;
                }

            }
            await next(context);

        }
    }
}
