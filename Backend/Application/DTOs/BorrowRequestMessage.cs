namespace Application.DTOs
{
    public class BorrowRequestMessage
    {
        public int BookId { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public int Duration { get; set; }
    }
}
