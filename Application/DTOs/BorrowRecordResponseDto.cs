using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record BorrowRecordResponseDto
    {
        public int Id { get; set; }
        public DateOnly BorrowDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public int BookId { get; set; }
        public string MemberId { get; set; }

    }
}
