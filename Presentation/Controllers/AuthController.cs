using Application.DTOs;
using Application.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Controller]
    public class AuthController :ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        [Route("api/login")]
        public async Task<IActionResult> Login([FromBody]LoginMemberDto loginMemberDto) {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var memberResponse = await authService.Login(loginMemberDto);
            if (memberResponse == null) return BadRequest("Wrong Username or Password");
            return Ok(memberResponse);
        }
        [HttpPost]
        [Route("api/register")]
        public async Task<IActionResult> Regiser([FromBody] RegisterMemberDto registerMemberDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            MemberResponseDto memberResponse = await authService.Signup(registerMemberDto);
            if (memberResponse == null) return BadRequest("This Email Is Already Taken");
            return Ok(memberResponse);
        }
        [HttpGet("/timeout")]
        public async Task<IActionResult> timeout(CancellationToken ct)
        {
                await Task.Delay(8000,ct);
                return Ok();
        }

        [HttpPost]
        [Authorize]
        [Route("api/refresh")]
        public async Task<IActionResult> refresh()
        {
            var user = User.Claims.FirstOrDefault(ct => ct.Type == ClaimTypes.Email);
            if (user == null) return BadRequest("refresh token is invalid");
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            string token = authHeader.Substring("Bearer ".Length).Trim();
            var ResponseDto = await authService.refresh(user.Value,token);
            if (ResponseDto == null) return BadRequest("that user doesn't exist");
            return Ok(ResponseDto);
        }
    }
}
