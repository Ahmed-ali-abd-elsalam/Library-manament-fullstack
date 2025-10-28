namespace Application.DTOs
{
    public record BorrowRecordResponseDto
    {
        public int Id { get; set; }
        public DateOnly? BorrowDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public int borrowDuration { get; set; }
        public string status { get; set; } = string.Empty;
        public int BookId { get; set; }
        public string MemberId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int StockCopies { get; set; }
        public int LateReturns { get; set; }
        public string BookTitle { get; set; } = string.Empty;

    }
}
