using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                MemberId = borrowRecord.MemberId,
                ReturnDate = borrowRecord.ReturnDate,
            };
        }
    }
}
