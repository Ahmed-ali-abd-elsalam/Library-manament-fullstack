using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record LoginResponseDto
    {
        public string Email { get; set; } = string.Empty;
        public string Response_Token { get; set; } = string.Empty;
        public string Refresh_token { get; set; } = string.Empty;
    }
}
