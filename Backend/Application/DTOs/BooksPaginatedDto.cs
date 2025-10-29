namespace Application.DTOs
{
    public class BooksPaginatedDto
    {
        public ICollection<BookResponseDto> Books { get; set; } = [];
        public int Total { get; set; }
        public int pageSize { get; set; }
        public int Offset { get; set; }
        public bool HasPrev { get; set; }
        public bool HasNext { get; set; }


    }
}
