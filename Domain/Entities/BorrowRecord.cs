namespace Domain.Entities
{
    public enum borrowStatus
    {
        Pending,
        Approved,
        Denied,
        Returned,
        Late
    }
    public class BorrowRecord
    {
        public int Id { get; set; }
        public DateOnly? BorrowDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public int borrowDuration { get; set; }
        public borrowStatus Status { get; set; }
        public Book Book { get; set; }
        public int BookId { get; set; }

        public Member Member { get; set; }
        public string MemberId { get; set; }
    }
}
