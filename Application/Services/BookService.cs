using Application.DTOs;
using Application.IRepository;
using Application.IService;
using Application.Mappers;
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
        public async Task<BooksPaginatedDto> GetAllBooks(int offset, int pageSize)
        {
            int total = await _repository.GetTotalCountAsync();
            var books = await _repository.GetBooksAsync(offset, pageSize);
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
        public async Task<BookResponseDto> AddNewBook(BookDto bookDto)
        {
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
        public async Task<BookResponseDto?> UpdateBook(BookDto bookDto,int bookId)
        {
            if(!await _repository.CheckExistsAsync(bookId))
            {
                return null;
            }
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
        public async Task<bool> DeleteBook(int bookId)
        {
            if (!await _repository.CheckExistsAsync(bookId))
            {
               throw new BookDoesnotExist($"no book exists with this ID {bookId}");
            }
            Book book = await _repository.GetBookAsync(bookId);
            return await _repository.DeleteBook(book);
        }
    }
}
