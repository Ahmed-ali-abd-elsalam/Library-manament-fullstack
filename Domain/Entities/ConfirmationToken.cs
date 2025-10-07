using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ConfirmationToken
    {
            public Guid id { get; set; }
            public string Email { get; set; } = string.Empty;
            public string Mode { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime expiresAt { get; set; }
    }
}
