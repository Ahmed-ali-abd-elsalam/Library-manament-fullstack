using Domain.Entities;

namespace Application.IRepository
{
    public interface IBorrowRecordRepository
    {
        Task<bool> CheckExistsAsync(int bookId, string MemberId);
        Task<bool> CheckExistsAsync(int BorrowRecordId);
        Task<BorrowRecord?> GetBorrowRecordAsync(int bookId, string MemberId);
        Task<BorrowRecord?> GetBorrowRecordAsync(int BorrowRecordId);
        Task<ICollection<BorrowRecord>> GetBorrowRecordsAsync(int offset = 0, int pagesize = 10);
        Task<int> getTotalCountAsync(string MemberId = "");
        public Task<ICollection<BorrowRecord>> GetBorrowRecordsAsync(string userId, int offset = 0, int pagesize = 10);
        Task<BorrowRecord> BorrowBookAsync(BorrowRecord borrowRecord);
        Task<BorrowRecord> editBorrowRecord(int id, BorrowRecord newBorrowRecord);
        Task<ICollection<BorrowRecord>> GetLateBorrowRecordsAsync();
        Task MarkRecordsAsLateAsync(List<int> recordIds);



    }
}
