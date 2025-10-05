using Application.DTOs;
using Application.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Controller]
    [Route("/api/[Controller]")]
    public class AuthController :ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginMemberDto loginMemberDto,CancellationToken cancellationToken) {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            string source = HttpContext.Request.Headers["User-Agent"];
            var memberResponse = await authService.Login(loginMemberDto,source,cancellationToken);
            if (memberResponse == null) return BadRequest("Wrong Username or Password");
            return Ok(memberResponse);
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterMemberDto registerMemberDto)
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
        [HttpDelete]
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            string email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null) return BadRequest("Invalid Token");
            string source= HttpContext.Request.Headers["User-Agent"];
            bool result = await authService.logOutAsync(email,source ,cancellationToken);
            if (!result) return BadRequest("User Not Found/Invalid Token");
            return Ok("Logged Out Successfully");
        }

        [HttpPost]
        [Authorize]
        [Route("refresh")]
        public async Task<IActionResult> refresh(CancellationToken cancellationToken)
        {
            string source = HttpContext.Request.Headers["User-Agent"];
            var user = User.Claims.FirstOrDefault(ct => ct.Type == ClaimTypes.Email);
            if (user == null) return BadRequest("refresh token is invalid");
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            string token = authHeader.Substring("Bearer ".Length).Trim();
            var ResponseDto = await authService.refresh(user.Value,token,source,cancellationToken);
            if (ResponseDto == null) return BadRequest("that user doesn't exist");
            return Ok(ResponseDto);
        }
        

        [HttpPut]
        [Route("forget-password")]
        //TODO seperate into 2 apis and send forget password link to email
        public async Task<IActionResult> forgotPassword([FromBody]ForgotPasswrodDTO forgotPasswordDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            bool result = await authService.forgotPassword(forgotPasswordDTO);
            if (!result) return BadRequest("invalid email / password");
            return Accepted();
        }

        [HttpGet]
        [Route("confirm-email")]
        public async Task<IActionResult> confirmemail(string Email,string TokenId)
        {
            if (!Guid.TryParse(TokenId, out Guid guid))
            {
                Console.WriteLine(TokenId);
                return BadRequest("Invalid token format.");
            }
            bool result = await authService.confirmEmail(Email, TokenId);
            if (!result) return BadRequest("Invalid Email/Token");
            return Accepted("Email Validated");
        }

        [HttpGet]
        public async Task<IActionResult> SendEmail(string Email)
        {
            await authService.SendEmail(Email);
            return Ok("Email Has been sent");
        }
    }
}
