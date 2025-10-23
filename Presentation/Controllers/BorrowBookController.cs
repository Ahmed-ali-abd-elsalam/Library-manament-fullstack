using Application.IService;
using Application.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Controller]
    public class BorrowBookController : ControllerBase
    {
        private readonly IBorrowRecordService borrowRecordService;

        public BorrowBookController(IBorrowRecordService borrowRecordService)
        {
            this.borrowRecordService = borrowRecordService;
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        [Route("/api/borrow/{BookId}")]
        public async Task<IActionResult> BorrowBook(int BookId)
        {
            string Email = User.FindFirst(ClaimTypes.Email)?.Value;
            var borrowRecordResult = await borrowRecordService.BorrowBook(BookId, Email);
            if (!borrowRecordResult.IsSuccess)
            {
                if (borrowRecordResult.error == Errors.DoesntExist) return NotFound(borrowRecordResult);
                if (borrowRecordResult.error == Errors.notAvailable) return BadRequest(borrowRecordResult);
            }
            return Ok(borrowRecordResult);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        [Route("/api/return/{BookId}")]
        public async Task<IActionResult> ReturnBook(int BookId)
        {
            string Email = User.FindFirst(ClaimTypes.Email)?.Value;
            var borrowRecordResult = await borrowRecordService.ReturnBook(BookId, Email);
            if (!borrowRecordResult.IsSuccess)
            {
                if (borrowRecordResult.error == Errors.DoesntExist) return NotFound(borrowRecordResult);
                if (borrowRecordResult.error == Errors.repeatedOperation) return Accepted(borrowRecordResult);
            }
            return Ok(borrowRecordResult);
        }

    }
}