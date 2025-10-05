using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string generateLink(string Mode, string Email,string TokenId = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if(Mode == "ConfirmEmail")
            {
                return _linkGenerator.GetUriByAction(
                    httpContext,
                    action: "confirmemail",
                    controller: "Auth",
                    values: new { Email, TokenId }
                    ) ?? string.Empty;
            }else if (Mode == "changPassword")
            {
                return _linkGenerator.GetUriByAction(
                                httpContext,
                                action: "forgot-password",
                                controller: "Auth",
                                values: new { Email }
                                ) ?? string.Empty;

            }
            return string.Empty;
        }
    }
}
