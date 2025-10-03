using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class Member :IdentityUser
    {
        public DateOnly JoinDate { get; set; }  = DateOnly.FromDateTime(DateTime.Now);
        public ICollection<BorrowRecord> BorrowRecords { get; set; } = [];
    }
}
