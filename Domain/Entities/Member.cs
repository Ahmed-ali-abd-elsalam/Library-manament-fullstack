namespace Domain.Entities
{
    public class Member
    {
        //TODO enforce role to be one of two values member and admin in DTOs
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role{ get; set; }
        public DateOnly JoinDate { get; set; }
        public ICollection<BorrowRecord> BorrowRecords { get; set; } = [];
    }
}
