using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Services
{
    public class LinkFactory
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LinkFactory(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public string generateLink(string Mode, string Email, string TokenId = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            string action = string.Empty;
            if (Mode == tokenModes.EmailValidation.ToString())
            {
                action = "confirmemail";
            }
            else if (Mode == tokenModes.PasswordReset.ToString())
            {
                action = "resetpassword";

            }
            return _linkGenerator.GetUriByAction(
                    httpContext,
                    action: action,
                    controller: "Auth",
                    values: new { Email, TokenId }
                    ) ?? string.Empty;

        }
    }
}
