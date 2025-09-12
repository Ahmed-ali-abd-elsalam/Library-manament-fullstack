using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    internal static class BookMapper
    {
        public static Book DtoToBookMapper(this BookDto dto)
        {
            return new Book
            {
                Author = dto.Author,
                Title = dto.Title,
                PublishedYear = dto.PublishedYear,
                IsAvailable = true
            };
        }
        public static BookResponseDto BookToDtoMapper(this Book book)
        {
            return new BookResponseDto
            {
                Author = book.Author,
                Title = book.Title,
                PublishedYear = book.PublishedYear,
                IsAvailable = book.IsAvailable
            };
        }
    }
}
