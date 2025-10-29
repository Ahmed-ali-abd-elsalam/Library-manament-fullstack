using Application.DTOs;
using Domain.Entities;

namespace Application.IRepository
{
    public interface IBookRepository
    {
        public Task<int> GetTotalCountAsync(BooksFilter booksFilter);
        public Task<bool> CheckExistsAsync(int Id);
        public Task<bool> CheckExistsAsync(string title);

        public Task<bool> CheckAvailableAsync(int Id);
        public Task<ICollection<Book>> GetBooksAsync(int offset, int pagesize, BooksFilter booksFilter);
        public Task<Book?> GetBookAsync(int Id);
        public Task<Book> AddBookAsync(Book book);
        public Task<Book> UpdateBookAsync(int Id, Book update);
        public Task DeleteBook(Book book);

    }
}
