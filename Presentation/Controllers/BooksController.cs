using Application.DTOs;
using Application.IService;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/books/")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        [ProducesResponseType(statusCode: 200, type: typeof(BooksPaginatedDto))]
        public async Task<IActionResult> getBooks([FromQuery] BooksFilter booksFilter,int offest = 0, int count = 100)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var booksResult = await _bookService.GetAllBooks(offest, count, booksFilter);
            return booksResult.IsSuccess? Ok(booksResult):NotFound(booksResult);
        }

        [HttpPost("/api/books/add")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(statusCode: 200,type: typeof(BookResponseDto))]
        public async Task<IActionResult> AddBook([FromBody]BookDto bookDto)

        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var bookResult = await _bookService.AddNewBook(bookDto);
            return bookResult.IsSuccess? Ok(bookResult) : BadRequest(bookResult);
        }
        
        [HttpPut("{BookId}")]
        [Authorize(Roles ="Admin")]
        [ProducesResponseType(statusCode: 200, type: typeof(BookResponseDto))]
        public async Task<IActionResult> UpdateBook(int BookId,BookDto bookDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var bookResult =await _bookService.UpdateBook(bookDto,BookId);
            return bookResult.IsSuccess ? Ok(bookResult) :NotFound(bookResult);
        }
        
        [HttpDelete("{BookId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(statusCode: 200, type: typeof(BookResponseDto))]
        public async Task<IActionResult> Delete(int BookId)
        {
                var bookResult = await _bookService.DeleteBook(BookId);
                return bookResult.IsSuccess?Accepted(bookResult): NotFound(bookResult);
        }

    }
}
