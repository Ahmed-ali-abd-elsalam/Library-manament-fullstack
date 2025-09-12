namespace Domain.Entities
{
    public class Member
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string HashedPassword { get; set; }
        public required string Role{ get; set; }
        public DateOnly JoinDate { get; set; }  = DateOnly.FromDateTime(DateTime.Now);
        public ICollection<BorrowRecord> BorrowRecords { get; set; } = [];
    }
}
