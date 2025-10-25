namespace Domain.Entities
{
    public enum borrowStatus
    {
        Pending,
        Approved,
        Denied,
        Removed,
        Returned,
        Borrowed

    }
    public class BorrowRecord
    {
        public int Id { get; set; }
        public DateOnly BorrowDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly? ReturnDate { get; set; }
        public Book Book { get; set; }
        public int BookId { get; set; }
        public borrowStatus Status { get; set; }
        public Member Member { get; set; }
        public string MemberId { get; set; }
    }
}
