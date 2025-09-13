using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IBookService
    {
        public Task<BooksPaginatedDto> GetAllBooks(int offset, int pageSize);
        public  Task<BookResponseDto> AddNewBook(BookDto bookDto);
        public Task<BookResponseDto?> UpdateBook(BookDto bookDto, int bookId);
        public Task<bool> DeleteBook(int bookId);



    }
}
