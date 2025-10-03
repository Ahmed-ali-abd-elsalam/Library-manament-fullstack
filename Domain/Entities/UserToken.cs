using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Index(nameof(token))]
    [Index(nameof(userEmail))]
    public class UserToken
    {
        public int Id { get; set; }
        public string token { get; set; } = string.Empty;
        public string userEmail { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string source { get; set; } = string.Empty;
        public Member user { get; set; }
        public string userId { get; set; }
    }
}
