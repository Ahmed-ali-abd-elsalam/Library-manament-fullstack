using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class Member :IdentityUser
    {
        public DateOnly JoinDate { get; set; }  = DateOnly.FromDateTime(DateTime.Now);
        public int LateReturns { get; set; } = 0;
        public string? RefreshToken { get; set; }
        public ICollection<BorrowRecord> BorrowRecords { get; set; } = [];
    }
}
