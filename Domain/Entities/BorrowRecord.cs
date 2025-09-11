using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BorrowRecord
    {
        public int Id { get; set; }
        public DateOnly BorrowDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public required Book Book { get; set; }
        public int BookId { get; set; }
        public required Member Member { get; set; }
        public int MemberId { get; set; }
    }
}
