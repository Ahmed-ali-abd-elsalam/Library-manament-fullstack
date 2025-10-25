using Application.IService;
using Application.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Controller]
    [Route("api/borrowBooks")]
    public class BorrowBookController : ControllerBase
    {
        private readonly IBorrowRecordService borrowRecordService;

        public BorrowBookController(IBorrowRecordService borrowRecordService)
        {
            this.borrowRecordService = borrowRecordService;
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        [Route("borrow/{BookId}")]
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
        [Route("return/{BookId}")]
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

        /**TODO add apis for 
            get self records
            get all records for admin auth
            get member records for admin auth
            get single record
            add remove dit single record
            and test them
        **/

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> myBorrowRecord(int offset = 0, int pagesize = 10)
        {
            string Email = User.FindFirst(ClaimTypes.Email)?.Value;
            var borrowRecordResult = await borrowRecordService.GetMemberBorrowRecords(Email, offset, pagesize);
            if (!borrowRecordResult.IsSuccess)
            {
                if (borrowRecordResult.error == Errors.DoesntExist) return NotFound(borrowRecordResult);
            }
            return Ok(borrowRecordResult);
        }

        [HttpGet("/{Email}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> memberBorrowRecord(string Email, int offset = 0, int pagesize = 10)
        {
            var borrowRecordResult = await borrowRecordService.GetMemberBorrowRecords(Email, offset, pagesize);
            if (!borrowRecordResult.IsSuccess)
            {
                if (borrowRecordResult.error == Errors.DoesntExist) return NotFound(borrowRecordResult);
            }
            return Ok(borrowRecordResult);
        }
        [HttpGet("/All")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllBorrowRecord(int offset = 0, int pagesize = 10)
        {
            var borrowRecordResult = await borrowRecordService.GetAllBorrowRecords(offset, pagesize);
            if (!borrowRecordResult.IsSuccess)
            {
                if (borrowRecordResult.error == Errors.DoesntExist) return NotFound(borrowRecordResult);
            }
            return Ok(borrowRecordResult);
        }

        [HttpGet("/{id}")]
        [Authorize]
        public async Task<IActionResult> singleBorrowRecord(int id)
        {
            //check if current user has admin role he can check any records else check if the record is owned by the logged in user
            var borrowRecordResult = await borrowRecordService.GetBorrowRecord(id, User);
            return !borrowRecordResult.IsSuccess ? Ok(borrowRecordResult) : NotFound(borrowRecordResult);
        }



    }
}