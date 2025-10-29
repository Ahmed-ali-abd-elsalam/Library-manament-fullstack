using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record BooksFilter
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateOnly PublishedYear { get; set; } = DateOnly.MinValue;
        public bool IsAvailable { get; set; } = true;
    }
}
