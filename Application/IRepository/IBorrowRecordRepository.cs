using Domain.Entities;

namespace Application.IRepository
{
    public interface IBorrowRecordRepository
    {
        Task<bool> CheckExistsAsync(int bookId, string MemberId);
        Task<BorrowRecord?> GetBorrowRecordAsync(int bookId, string MemberId);
        Task<ICollection<BorrowRecord>> GetBorrowRecordsAsync(int offset = 0, int pagesize = 10);
        Task<int> getTotalCountAsync(string MemberId = "");
        public Task<ICollection<BorrowRecord>> GetBorrowRecordsAsync(string userId, int offset = 0, int pagesize = 10);
        Task<BorrowRecord> BorrowBookAsync(BorrowRecord borrowRecord);
        Task<BorrowRecord> ReturnBookAsync(int borrowRecordId, DateOnly returnDate);

    }
}
