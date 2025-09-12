using Application.IRepository;
using Domain.Entities;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BorrowRecordService
    {
        private readonly IBorrowRecordRepository _repository;
        private readonly IBookRepository _bookrepository;
        private readonly IMemberRepository _memberrepository;

        public BorrowRecordService(IBorrowRecordRepository repository, IBookRepository bookrepository, IMemberRepository memberrepository)
        {
            _repository = repository;
            _bookrepository = bookrepository;
            _memberrepository = memberrepository;
        }

        // Refactor this to be single responsiblity
        public async Task<BorrowRecord?> BorrowBook(int bookID,int userId)
        {
            bool bookExists = await _bookrepository.CheckExistsAsync(bookID);
            if (!bookExists) throw new BookDoesnotExist($"No Book Exists with this ID {bookID}");
            bool bookAvailable = await _bookrepository.CheckAvailableAsync(bookID);
            if (!bookAvailable) return null;
            Book book = await _bookrepository.GetBookAsync(bookID);
            book.IsAvailable = false;
            await _bookrepository.UpdateBookAsync(bookID, book);
            Member user = await _memberrepository.GetMemberAsync(userId);
            BorrowRecord borrowRecord = new BorrowRecord {
                BookId = bookID ,
                MemberId =userId,
                Member = user,
                Book=book,
                BorrowDate=DateOnly.FromDateTime(DateTime.UtcNow)
            };
            return await _repository.BorrowBookAsync(borrowRecord);
        }
        public async Task<BorrowRecord?> ReturnBook(int bookID,int userId)
        {
            bool recordExists = await _repository.CheckExistsAsync(bookID, userId);
            if (!recordExists) return null;
            BorrowRecord borrowRecord = await _repository.GetBorrowRecordAsync(bookID, userId);
            Book book = await _bookrepository.GetBookAsync(bookID);
            book.IsAvailable = true;
            await _bookrepository.UpdateBookAsync(bookID, book);
            return await _repository.ReturnBookAsync(borrowRecord.Id,DateOnly.FromDateTime(DateTime.UtcNow));


        }
    }
}
