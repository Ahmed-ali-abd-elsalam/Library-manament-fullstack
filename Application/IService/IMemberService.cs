using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IMemberService
    {
        public  Task<ICollection<MemberResponseDto>> GetMembers();
        public  Task<MemberResponseDto> AddMember(RegisterMemberDto memberDto);


    }
}
