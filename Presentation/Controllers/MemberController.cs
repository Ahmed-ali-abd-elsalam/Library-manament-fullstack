using Application.DTOs;
using Application.IService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Controller]
    [Route("api/[Controller]")]
    public class MemberController :ControllerBase
    {
        private readonly IMemberService memberService;

        public MemberController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<MemberResponseDto>))]
        public async Task<IActionResult> getAllMembers()
        {
            return Ok(await memberService.GetMembers());
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MemberResponseDto))]
        public async Task<IActionResult> AddMember([FromBody] RegisterMemberDto memberDto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await memberService.AddMember(memberDto));
        }
    }
}
