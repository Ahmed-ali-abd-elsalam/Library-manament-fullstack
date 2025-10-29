using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public record BookDto
    {
        [Required]
        public required string Author { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [NotInFuture]
        public DateOnly PublishedYear { get; set; }
        [Required]
        [GreaterThan(0)]
        public int Copies { get; set; }


    }
}
