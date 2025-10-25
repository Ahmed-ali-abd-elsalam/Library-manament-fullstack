using Application.DTOs;
using Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Controller]
    [Route("api/members/")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService memberService;

        public MemberController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> getAllMembers(MembersFilter membersFilter, int offset = 0, int pagesize = 20)
        {
            return Ok(await memberService.GetMembers(offset, pagesize, membersFilter));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddMember([FromBody] RegisterMemberDto memberDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await memberService.AddMember(memberDto));
        }
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> getMember()
        {
            string email = User.FindFirst(ClaimTypes.Email).Value;
            var memberResult = await memberService.GetMember(email);
            return memberResult.IsSuccess ? Ok(memberResult) : NotFound(memberResult);
        }

        [HttpGet("member")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> getMember(string Email)
        {
            var memberResult = await memberService.GetMember(Email);
            return memberResult.IsSuccess ? Ok(memberResult) : NotFound(memberResult);
        }
    }
}
