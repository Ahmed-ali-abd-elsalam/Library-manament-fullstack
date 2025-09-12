using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepository
{
    public interface IBookRepository
    {
        public Task<int> GetTotalCountAsync();
        public Task<bool> CheckExistsAsync(int Id);
        public Task<bool> CheckAvailableAsync(int Id);
        public Task<ICollection<Book>> GetBooksAsync(int offset,int pagesize);
        public Task<Book?> GetBookAsync(int Id);
        public Task<Book> AddBookAsync(Book book);
        public Task<Book> UpdateBookAsync(int Id, Book update);
        public Task<bool> DeleteBook(Book book);
        
    }
}
