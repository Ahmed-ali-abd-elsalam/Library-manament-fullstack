
namespace Domain.Entities
{
    public class Book
    {
        public int Id{ get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public DateOnly PublishedYear { get; set; }
        public bool IsAvailable { get; set; } = true;
        public ICollection<BorrowRecord> BorrowRecords { get; set; } = [];
    }
}
