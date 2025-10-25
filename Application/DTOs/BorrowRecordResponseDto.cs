namespace Application.DTOs
{
    public record BorrowRecordResponseDto
    {
        public int Id { get; set; }
        public DateOnly BorrowDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public string status { get; set; }
        public int BookId { get; set; }
        public string MemberId { get; set; }

    }
}
