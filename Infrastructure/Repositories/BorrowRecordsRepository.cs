using Application.IRepository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BorrowRecordsRepository : IBorrowRecordRepository
    {
        private readonly LibraryDbContext _context;

        public BorrowRecordsRepository(LibraryDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckExistsAsync(int bookId, string MemberId)
        {
            return await _context.BorrowRecords.AsNoTracking().AnyAsync(BR => BR.BookId == bookId && BR.MemberId == MemberId);
        }
        public async Task<bool> CheckExistsAsync(int BorrowRecordId)
        {
            return await _context.BorrowRecords.AsNoTracking().AnyAsync(BR => BR.Id == BorrowRecordId);
        }

        public async Task<BorrowRecord?> GetBorrowRecordAsync(int bookId, string MemberId)
        {
            return await _context.BorrowRecords.Include(Br => Br.Member).Include(Br => Br.Book).AsNoTracking().OrderBy(BR => BR.BookId).OrderBy(Br => Br.Id).FirstOrDefaultAsync(BR => BR.BookId == bookId && BR.MemberId == MemberId);
        }
        public async Task<BorrowRecord?> GetBorrowRecordAsync(int BorrowRecordId)
        {
            return await _context.BorrowRecords.Include(Br => Br.Member).Include(Br => Br.Book).AsNoTracking().FirstOrDefaultAsync(BR => BR.Id == BorrowRecordId);
        }

        public async Task<ICollection<BorrowRecord>> GetBorrowRecordsAsync(int offset, int pagesize)
        {
            return await _context.BorrowRecords.OrderBy(BR => BR.BookId).OrderBy(Br => Br.Id).Skip(offset * pagesize).Take(pagesize).ToListAsync();
        }
        public async Task<ICollection<BorrowRecord>> GetBorrowRecordsAsync(string Id, int offset, int pagesize)
        {
            return await _context.BorrowRecords.Where(br => br.MemberId == Id).Skip(offset * pagesize).Take(pagesize).ToListAsync();
        }
        public async Task<BorrowRecord> BorrowBookAsync(BorrowRecord borrowRecord)
        {
            await _context.BorrowRecords.AddAsync(borrowRecord);
            return borrowRecord;
        }
        //Refactor
        //public async Task<BorrowRecord> ReturnBookAsync(int borrowRecordId, DateOnly returnDate)
        //{
        //    BorrowRecord borrowRecord = await _context.BorrowRecords.FirstOrDefaultAsync(BR => BR.Id == borrowRecordId);
        //    borrowRecord.ReturnDate = returnDate;
        //    return borrowRecord;
        //}

        public Task<int> getTotalCountAsync(string MemberId = "")
        {
            if (MemberId == "")
                return _context.BorrowRecords.CountAsync();
            return _context.BorrowRecords.Where(Br => Br.MemberId == MemberId).CountAsync();
        }

        public async Task<BorrowRecord> editBorrowRecord(int id, BorrowRecord newBorrowRecord)
        {
            BorrowRecord borrowRecord = await _context.BorrowRecords.FirstOrDefaultAsync(BR => BR.Id == id);
            borrowRecord = newBorrowRecord;
            _context.BorrowRecords.Update(borrowRecord);
            return borrowRecord;

        }
    }
}
