namespace Application.DTOs
{
    public class PaginatedBorrowRecordResponseDto
    {
        public ICollection<BorrowRecordResponseDto> BorrowRecords { get; set; } = [];
        public int Total { get; set; }
        public int pageSize { get; set; }
        public int Offset { get; set; }
        public bool HasPrev { get; set; }
        public bool HasNext { get; set; }
    }
}
