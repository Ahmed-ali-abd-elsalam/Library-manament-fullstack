using Application.DTOs;
using Application.IRepository;
using Application.IService;
using Application.Mappers;
using Application.Results;
using Domain.Entities;
using System.Security.Claims;

namespace Application.Services
{
    public class BorrowRecordService : IBorrowRecordService
    {
        private readonly IBorrowRecordRepository _repository;
        private readonly IBookRepository _bookrepository;
        private readonly IMemberRepository _memberrepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly INotificationService _notifier;


        public BorrowRecordService(IBorrowRecordRepository repository, IBookRepository bookrepository, IMemberRepository memberrepository, IUnitOfWork unitOfWork, INotificationService notifier)
        {
            _repository = repository;
            _bookrepository = bookrepository;
            _memberrepository = memberrepository;
            this.unitOfWork = unitOfWork;
            _notifier = notifier;
        }

        public async Task<Result<BorrowRecordResponseDto>> BorrowBook(int bookID, ClaimsPrincipal User, int borrowDuration)
        {
            string UserId = User.FindFirstValue("Id");
            string userEmail = User.FindFirstValue(ClaimTypes.Email);
            var borrowRecord = await _repository.GetBorrowRecordAsync(bookID, UserId);
            if (borrowRecord is not null && (borrowRecord.Status != borrowStatus.Returned && borrowRecord.Status != borrowStatus.Denied)) return Errors.RepeatedOperation;
            bool bookExists = await _bookrepository.CheckExistsAsync(bookID);
            if (!bookExists) return Errors.DoesntExist(typeof(Book).Name);
            bool bookAvailable = await _bookrepository.CheckAvailableAsync(bookID);
            if (!bookAvailable) return Errors.NotAvailable;
            Book book = await _bookrepository.GetBookAsync(bookID);
            Member user = await _memberrepository.GetMemberAsyncByEmail(userEmail);
            borrowRecord = await _repository.BorrowBookAsync(new BorrowRecord
            {
                BookId = bookID,
                MemberId = user.Id,
                Member = user,
                Book = book,
                Status = borrowStatus.Pending,
                borrowDuration = borrowDuration
            });
            await unitOfWork.SaveChangesAsync();
            await _notifier.SendToAdminsAsync("a new Borrow Request Recieved");
            return borrowRecord.BorrowRecordtoDto();
        }
        public async Task<Result<BorrowRecordResponseDto>> ReturnBook(int bookID, string userId)
        {
            if (!await _repository.CheckExistsAsync(bookID, userId))
                return Errors.DoesntExist(typeof(Member).Name);
            BorrowRecord borrowRecord = await _repository.GetBorrowRecordAsync(bookID, userId);
            if (borrowRecord.Status == borrowStatus.Returned) return Errors.RepeatedOperation;
            if (borrowRecord.Status == borrowStatus.Pending || borrowRecord.Status == borrowStatus.Denied) return Errors.InvalidOperation;
            Book book = await _bookrepository.GetBookAsync(bookID);
            book.Copies += 1;
            await _bookrepository.UpdateBookAsync(bookID, book);
            borrowRecord.ReturnDate = DateOnly.FromDateTime(DateTime.UtcNow);
            borrowRecord.Status = borrowStatus.Returned;
            borrowRecord = await _repository.editBorrowRecord(borrowRecord.Id, borrowRecord);
            await unitOfWork.SaveChangesAsync();
            return borrowRecord.BorrowRecordtoDto();
        }

        public async Task<Result<PaginatedBorrowRecordResponseDto>> GetMemberBorrowRecords(string Email, int offset, int pagesize)
        {
            if (!await _memberrepository.CheckExistsAsyncByEmail(Email)) return Errors.DoesntExist(typeof(Member).Name);
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

        public async Task<Result<BorrowRecordResponseDto>> GetBorrowRecord(int id, ClaimsPrincipal User)
        {
            // if admin return BR // if owner return BR // if not owner return not owned // if BR doesnt exist
            var email = User.FindFirst(ClaimTypes.Email);
            if (email == null) return Errors.InvalidToken;
            Member member = await _memberrepository.GetMemberAsyncByEmail(email.Value);
            if (member == null) return Errors.DoesntExist(typeof(Member).Name);
            bool isAdmin = User.IsInRole("Admin");
            if (!await _repository.CheckExistsAsync(id)) return Errors.DoesntExist(typeof(BorrowRecord).Name);
            var borrowRecord = await _repository.GetBorrowRecordAsync(id);
            if (borrowRecord.MemberId != member.Id & !isAdmin) return Errors.DoesntBelong;
            return borrowRecord.BorrowRecordtoDto();
        }

        public async Task<Result<BorrowRecordResponseDto>> HandleBorrowRequests(int id, string status)
        {
            string[] adminStatuses = ["Approved", "Denied"];
            if (!await _repository.CheckExistsAsync(id)) return Errors.DoesntExist(typeof(BorrowRecord).Name);
            var borrowRecord = await _repository.GetBorrowRecordAsync(id);
            if (!Enum.TryParse<borrowStatus>(status, true, out var newStatus)) return Errors.InvalidInputs;
            if (newStatus == borrowStatus.Approved)
            {
                Book book = await _bookrepository.GetBookAsync(borrowRecord.BookId);
                if (book == null) return Errors.DoesntExist(typeof(Book).Name);
                book.Copies -= 1;
                if (book.Copies < 0) return Errors.CantApprove;
                borrowRecord.BorrowDate = DateOnly.FromDateTime(DateTime.UtcNow);
                borrowRecord.ReturnDate = DateOnly.FromDateTime(
                    DateTime.UtcNow.AddDays(borrowRecord.borrowDuration));
                await _bookrepository.UpdateBookAsync(borrowRecord.BookId, book);
                //TODO notify User
            }
            await _notifier.SendToUserAsync(borrowRecord.MemberId, $"your borrowRequest was {status}");
            borrowRecord.Status = newStatus;
            await _repository.editBorrowRecord(id, borrowRecord);
            await unitOfWork.SaveChangesAsync();
            return borrowRecord.BorrowRecordtoDto();
        }
    }
}
