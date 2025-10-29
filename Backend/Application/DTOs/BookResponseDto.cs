namespace Application.DTOs
{
    public record BookResponseDto
    {
        public int Id { get; set; }
        public required string Author { get; set; }
        public required string Title { get; set; }
        public DateOnly PublishedYear { get; set; }
        public int Copies { get; set; }

    }
}
