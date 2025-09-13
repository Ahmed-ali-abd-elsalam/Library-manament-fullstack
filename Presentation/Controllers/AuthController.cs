using Application.DTOs;
using Application.IService;
using Microsoft.AspNetCore.Mvc;
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
            string memberResponse = await authService.Login(loginMemberDto);
            if (memberResponse == null) return BadRequest("Wrong Username or Passwrd");
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
    }
}
