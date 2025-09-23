using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepository
{
    public interface IBorrowRecordRepository
    {
        Task<bool> CheckExistsAsync(int bookId, string UserId);
        Task<BorrowRecord?> GetBorrowRecordAsync(int bookId, string UserId);
        Task<ICollection<BorrowRecord>> GetBorrowRecordsAsync();
        Task<BorrowRecord> BorrowBookAsync(BorrowRecord borrowRecord);
        Task<BorrowRecord> ReturnBookAsync(int borrowRecordId, DateOnly returnDate);
    }
}
