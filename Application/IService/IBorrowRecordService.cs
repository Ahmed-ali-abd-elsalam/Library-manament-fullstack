using Application.DTOs;
using Application.Results;
using System.Security.Claims;

namespace Application.IService
{
    public interface IBorrowRecordService
    {
        public Task<Result<BorrowRecordResponseDto>> BorrowBook(int bookID, string userEmail);
        public Task<Result<BorrowRecordResponseDto>> ReturnBook(int bookID, string userEmail);
        public Task<Result<PaginatedBorrowRecordResponseDto>> GetMemberBorrowRecords(string Email, int offset, int pagesize);
        public Task<Result<PaginatedBorrowRecordResponseDto>> GetAllBorrowRecords(int offset, int pagesize);
        public Task<Result<BorrowRecordResponseDto>> GetBorrowRecord(int id, ClaimsPrincipal User);



    }
}
