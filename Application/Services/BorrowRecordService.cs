using Application.DTOs;
using Application.IRepository;
using Application.IService;
using Application.Mappers;
using Application.Results;
using Domain.Entities;

namespace Application.Services
{
    public class BorrowRecordService : IBorrowRecordService
    {
        private readonly IBorrowRecordRepository _repository;
        private readonly IBookRepository _bookrepository;
        private readonly IMemberRepository _memberrepository;
        private readonly IUnitOfWork unitOfWork;

        public BorrowRecordService(IBorrowRecordRepository repository, IBookRepository bookrepository, IMemberRepository memberrepository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _bookrepository = bookrepository;
            _memberrepository = memberrepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result<BorrowRecordResponseDto>> BorrowBook(int bookID, string userEmail)
        {
            bool bookExists = await _bookrepository.CheckExistsAsync(bookID);
            if (!bookExists) return Errors.DoesntExist;
            bool bookAvailable = await _bookrepository.CheckAvailableAsync(bookID);
            if (!bookAvailable) return Errors.notAvailable;
            Book book = await _bookrepository.GetBookAsync(bookID);
            book.IsAvailable = false;
            await _bookrepository.UpdateBookAsync(bookID, book);
            Member user = await _memberrepository.GetMemberAsyncByEmail(userEmail);
            BorrowRecord borrowRecord = await _repository.BorrowBookAsync(new BorrowRecord
            {
                BookId = bookID,
                MemberId = user.Id,
                Member = user,
                Book = book,
                Status = borrowStatus.Pending,
                BorrowDate = DateOnly.FromDateTime(DateTime.UtcNow)
            });
            await unitOfWork.SaveChangesAsync();
            return borrowRecord.BorrowRecordtoDto();
        }
        public async Task<Result<BorrowRecordResponseDto>> ReturnBook(int bookID, string userEmail)
        {
            Member user = await _memberrepository.GetMemberAsyncByEmail(userEmail);
            if (!await _repository.CheckExistsAsync(bookID, user.Id))
                return Errors.DoesntExist;
            BorrowRecord borrowRecord = await _repository.GetBorrowRecordAsync(bookID, user.Id);
            if (borrowRecord.ReturnDate != null) return Errors.repeatedOperation;
            Book book = await _bookrepository.GetBookAsync(bookID);
            book.IsAvailable = true;
            await _bookrepository.UpdateBookAsync(bookID, book);
            borrowRecord = await _repository.ReturnBookAsync(borrowRecord.Id, DateOnly.FromDateTime(DateTime.UtcNow));
            await unitOfWork.SaveChangesAsync();
            return borrowRecord.BorrowRecordtoDto();
        }

        public async Task<Result<PaginatedBorrowRecordResponseDto>> GetMemberBorrowRecords(string Email, int offset, int pagesize)
        {
            if (await _memberrepository.CheckExistsAsyncByEmail(Email) != null) return Errors.DoesntExist;
            Member member = await _memberrepository.GetMemberAsyncByEmail(Email);
            int total = await _repository.getTotalCountAsync(member.Id);
            bool HasNext = offset + 1 * pagesize < total;
            bool HasPrev = offset > 0;
            var borrowRecords = await _repository.GetBorrowRecordsAsync(member!.Id, offset, pagesize);
            List<BorrowRecordResponseDto> borrowRecordResponseDtos = [];
            foreach (var borrowrecord in borrowRecords)
            {
                borrowRecordResponseDtos.Add(borrowrecord.BorrowRecordtoDto());
            }
            return new PaginatedBorrowRecordResponseDto
            {
                BorrowRecords = borrowRecordResponseDtos,
                Total = total,
                HasNext = HasNext,
                HasPrev = HasPrev,
                Offset = offset,
                pageSize = pagesize
            };
        }

        public async Task<Result<PaginatedBorrowRecordResponseDto>> GetAllBorrowRecords(int offset, int pagesize)
        {
            int total = await _repository.getTotalCountAsync();
            bool HasNext = offset + 1 * pagesize < total;
            bool HasPrev = offset > 0;
            var borrowRecords = await _repository.GetBorrowRecordsAsync(offset, pagesize);
            List<BorrowRecordResponseDto> borrowRecordResponseDtos = [];
            foreach (var borrowrecord in borrowRecords)
            {
                borrowRecordResponseDtos.Add(borrowrecord.BorrowRecordtoDto());
            }
            return new PaginatedBorrowRecordResponseDto
            {
                BorrowRecords = borrowRecordResponseDtos,
                Total = total,
                HasNext = HasNext,
                HasPrev = HasPrev,
                Offset = offset,
                pageSize = pagesize
            };
        }

    }
}
