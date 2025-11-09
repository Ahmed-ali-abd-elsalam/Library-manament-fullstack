using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers
{
    internal static class BorrowRecordMapper
    {
        public static BorrowRecordResponseDto BorrowRecordtoDto(this BorrowRecord borrowRecord)
        {
            return new BorrowRecordResponseDto
            {
                Id = borrowRecord.Id,
                BookId = borrowRecord.BookId,
                BorrowDate = borrowRecord.BorrowDate,
                borrowDuration = borrowRecord.borrowDuration,
                status = borrowRecord.Status.ToString(),
                MemberId = borrowRecord.MemberId,
                ReturnDate = borrowRecord.ReturnDate,
                LateReturns = borrowRecord.Member.LateReturns,
                Email = borrowRecord.Member.Email,
                StockCopies = borrowRecord.Book.Copies,
                BookTitle = borrowRecord.Book.Title,
            };
        }
    }
}
