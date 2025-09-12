using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
