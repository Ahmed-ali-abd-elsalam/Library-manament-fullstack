using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record BookDto
    {
        [Required]
        public required string Author { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]        
        public DateOnly PublishedYear { get; set; }

    }
}
