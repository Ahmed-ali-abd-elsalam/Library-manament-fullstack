using Application.DTOs;
using Application.IRepository;
using Application.IService;
using Application.Mappers;
using Application.Results;
using Domain.Entities;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BookService :IBookService
    {
        private readonly IBookRepository _repository;

        public BookService(IBookRepository repository)
        {
            _repository = repository;
        }
        public async Task<Result<BooksPaginatedDto>> GetAllBooks(int offset, int pageSize, BooksFilter booksFilter)
        {
            int total = await _repository.GetTotalCountAsync(booksFilter);
            var books = await _repository.GetBooksAsync(offset, pageSize,booksFilter);
            var booksDtos = new List<BookResponseDto>();
            foreach (var book in books)
            {
                booksDtos.Add(book.BookToDtoMapper());
            }
            bool HasNext = offset+1 * pageSize < total;
            bool HasPrev = offset > 0;
            return new BooksPaginatedDto { Books = booksDtos,
                Total = total,
                HasNext = HasNext,
                HasPrev = HasPrev,
                Offset = offset,
                pageSize = pageSize };
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
            return book.BookToDtoMapper();
        }
        public async Task<Result<BookResponseDto?>> UpdateBook(BookDto bookDto,int bookId)
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
            book = await _repository.UpdateBookAsync(bookId,book);
            return book.BookToDtoMapper();
        }
        public async Task<Result> DeleteBook(int bookId)
        {
            if (!await _repository.CheckExistsAsync(bookId))
                return Result.Fail(Errors.DoesntExist);
            Book book = await _repository.GetBookAsync(bookId);
            return await _repository.DeleteBook(book)?Result.success() : Result.Fail(Errors.DeletionFailed);
        }
    }
}
