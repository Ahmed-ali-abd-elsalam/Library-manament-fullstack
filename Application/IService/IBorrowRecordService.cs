using Application.DTOs;
using Application.Results;

namespace Application.IService
{
    public interface IBorrowRecordService
    {
        public Task<Result<BorrowRecordResponseDto>> BorrowBook(int bookID, string userEmail);
        public Task<Result<BorrowRecordResponseDto?>> ReturnBook(int bookID, string userEmail);


    }
}
