using Application.DTOs;
using Domain.Entities;

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
                Copies = dto.Copies
            };
        }
        public static BookResponseDto BookToDtoMapper(this Book book)
        {
            return new BookResponseDto
            {
                Id = book.Id,
                Author = book.Author,
                Title = book.Title,
                PublishedYear = book.PublishedYear,
                Copies = book.Copies
            };
        }
    }
}
