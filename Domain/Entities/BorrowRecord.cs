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
        public DateOnly BorrowDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly? ReturnDate { get; set; }
        public Book Book { get; set; }
        public int BookId { get; set; }
        public Member Member { get; set; }
        public string MemberId { get; set; }
    }
}
