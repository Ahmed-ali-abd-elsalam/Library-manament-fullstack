using Application.DTOs;
using Application.IRepository;
using Application.IService;
using Application.Mappers;
using Application.Results;
using Domain.Entities;

namespace Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repository;
        private readonly IUnitOfWork unitOfWork;

        public BookService(IBookRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Result<BooksPaginatedDto>> GetAllBooks(int offset, int pageSize, BooksFilter booksFilter)
        {
            int total = await _repository.GetTotalCountAsync(booksFilter);
            var books = await _repository.GetBooksAsync(offset, pageSize, booksFilter);
            var booksDtos = new List<BookResponseDto>();
            foreach (var book in books)
            {
                booksDtos.Add(book.BookToDtoMapper());
            }
            bool HasNext = offset + 1 * pageSize < total;
            bool HasPrev = offset > 0;
            return new BooksPaginatedDto
            {
                Books = booksDtos,
                Total = total,
                HasNext = HasNext,
                HasPrev = HasPrev,
                Offset = offset,
                pageSize = pageSize
            };
        }
        public async Task<Result<BookResponseDto>> AddNewBook(BookDto bookDto)
        {
            if (await _repository.CheckExistsAsync(bookDto.Title))
                return Errors.duplicateEntry;
            Book book = new Book
            {
                Author = bookDto.Author,
                Title = bookDto.Title,
                PublishedYear = bookDto.PublishedYear,
                IsAvailable = true
            };
            book = await _repository.AddBookAsync(book);
            await unitOfWork.SaveChangesAsync();
            return book.BookToDtoMapper();
        }
        public async Task<Result<BookResponseDto?>> UpdateBook(BookDto bookDto, int bookId)
        {
            if (!await _repository.CheckExistsAsync(bookId))
                return Errors.DoesntExist;
            Book book = new Book
            {
                Author = bookDto.Author,
                Title = bookDto.Title,
                PublishedYear = bookDto.PublishedYear,
                IsAvailable = true
            };
            book = await _repository.UpdateBookAsync(bookId, book);
            await unitOfWork.SaveChangesAsync();
            return book.BookToDtoMapper();
        }
        public async Task<Result> DeleteBook(int bookId)
        {
            Book? book = await _repository.GetBookAsync(bookId);
            if (book is null)
                return Result.Fail(Errors.DoesntExist);
            await _repository.DeleteBook(book);
            await unitOfWork.SaveChangesAsync();
            return Result.success();
        }

        public async Task<Result<BookResponseDto>> GetBook(int BookId)
        {
            Book? book = await _repository.GetBookAsync(BookId);
            return book is null ? book!.BookToDtoMapper() : Errors.DoesntExist;
        }
    }
}
