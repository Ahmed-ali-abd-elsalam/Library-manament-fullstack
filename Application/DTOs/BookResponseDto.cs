using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record BookResponseDto
    {
        public int Id { get; set; }
        public required string Author { get; set; }
        public required string Title { get; set; }
        public DateOnly PublishedYear { get; set; }
        public bool IsAvailable { get; set; }

    }
}
