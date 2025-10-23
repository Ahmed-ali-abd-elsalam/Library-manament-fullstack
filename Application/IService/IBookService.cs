using Application.DTOs;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IBookService
    {
        public Task<Result<BooksPaginatedDto>> GetAllBooks(int offset, int pageSize, BooksFilter booksFilter);
        public  Task<Result<BookResponseDto>> AddNewBook(BookDto bookDto);
        public Task<Result<BookResponseDto?>> UpdateBook(BookDto bookDto, int bookId);
        public Task<Result> DeleteBook(int bookId);



    }
}
