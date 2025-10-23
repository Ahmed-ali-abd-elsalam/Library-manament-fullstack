﻿using Application.DTOs;
using Application.IRepository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;
        public BookRepository(LibraryDbContext context)
        {
            _context = context;
        }
        public async Task<Book> AddBookAsync(Book book)
        {
             await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> CheckAvailableAsync(int Id)
        {
            Book book = await _context.Books.FirstOrDefaultAsync(b => b.Id == Id);
            return book.IsAvailable;
        }

        public async Task<bool> CheckExistsAsync(int Id)
        {
            return await _context.Books.AnyAsync(B => B.Id == Id);
        }

        public async Task<bool> CheckExistsAsync(string title)
        {
            return await _context.Books.AnyAsync(B => B.Title == title);
        }

        public async Task<bool>DeleteBook(Book book)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Book?> GetBookAsync(int Id)
        {
            return await _context.Books.FirstOrDefaultAsync(B => B.Id == Id);
        }

        public async Task<ICollection<Book>> GetBooksAsync(int offset, int pagesize, BooksFilter booksFilter)
        {
            var query =  _context.Books.AsQueryable();
            query = query.Where(book => book.IsAvailable == booksFilter.IsAvailable);
            if(booksFilter.Title != string.Empty) 
                query = query.Where(book => book.Title == booksFilter.Title);
            if(booksFilter.Author != string.Empty)
                query = query.Where(book => book.Author == booksFilter.Author);
            if(booksFilter.PublishedYear != DateOnly.MinValue)
                query = query.Where(book => book.PublishedYear == booksFilter.PublishedYear);
            return await query.OrderBy(b=>b.Id).Skip(offset*pagesize).Take(pagesize).ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(BooksFilter booksFilter)
        {
            var query = _context.Books.AsQueryable();
            query = query.Where(book => book.IsAvailable == booksFilter.IsAvailable);
            if (booksFilter.Title != string.Empty)
                query = query.Where(book => book.Title == booksFilter.Title);
            if (booksFilter.Author != string.Empty)
                query = query.Where(book => book.Author == booksFilter.Author);
            if (booksFilter.PublishedYear != DateOnly.MinValue)
                query = query.Where(book => book.PublishedYear == booksFilter.PublishedYear);
            return await query.CountAsync();
        }

        public async Task<Book> UpdateBookAsync(int Id, Book update)
        {
            Book book = await _context.Books.FirstOrDefaultAsync(b => b.Id == Id);
            book.Title = update.Title;
            book.Author = update.Author;
            book.PublishedYear = update.PublishedYear;
            await _context.SaveChangesAsync();
            return book;
        }
    }
}
