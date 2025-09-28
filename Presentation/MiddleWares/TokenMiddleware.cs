using Application.IService;
using Domain.Entities;

namespace Presentation.MiddleWares
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated == true)
            {
                string authHeader = context.Request.Headers["Authorization"];
                string token = authHeader.Substring("Bearer ".Length).Trim();
                var tokenService = context.RequestServices.GetRequiredService<IUserTokenService>();
                UserToken userToken = await tokenService.getTokenAsync(token);
                if (userToken == null)
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
