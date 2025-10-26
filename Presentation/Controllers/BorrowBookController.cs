using Application.IService;
using Application.Results;
using Domain.Entities;
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
                if (borrowRecordResult.error == Errors.DoesntExist(typeof(Book).Name)) return NotFound(borrowRecordResult);
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
                if (borrowRecordResult.error == Errors.DoesntExist(typeof(Member).Name)) return NotFound(borrowRecordResult);
                if (borrowRecordResult.error == Errors.repeatedOperation) return Accepted(borrowRecordResult);
            }
            return Ok(borrowRecordResult);
        }

        /**TODO add apis for 
            
            add return date and borrow duration to database and borrow controller 
            get self records        done
            get all records for admin auth      done
            get member records for admin auth       done
            get single record       done
            add remove dit single record        planned
            and test them partially
            
        **/

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> myBorrowRecord(int offset = 0, int pagesize = 10)
        {
            string Email = User.FindFirst(ClaimTypes.Email)?.Value;
            var borrowRecordResult = await borrowRecordService.GetMemberBorrowRecords(Email, offset, pagesize);
            if (!borrowRecordResult.IsSuccess)
            {
                if (borrowRecordResult.error == Errors.DoesntExist(typeof(Member).Name)) return NotFound(borrowRecordResult);
            }
            return Ok(borrowRecordResult);
        }

        [HttpGet("member/{Email}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> memberBorrowRecords(string Email, int offset = 0, int pagesize = 10)
        {
            var borrowRecordResult = await borrowRecordService.GetMemberBorrowRecords(Email, offset, pagesize);
            if (!borrowRecordResult.IsSuccess)
            {
                if (borrowRecordResult.error == Errors.DoesntExist(typeof(Member).Name)) return NotFound(borrowRecordResult);
            }
            return Ok(borrowRecordResult);
        }
        [HttpGet("All")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllBorrowRecords(int offset = 0, int pagesize = 10)
        {
            var borrowRecordResult = await borrowRecordService.GetAllBorrowRecords(offset, pagesize);
            //if (!borrowRecordResult.IsSuccess)
            //{
            //    if (borrowRecordResult.error == Errors.DoesntExist) return NotFound(borrowRecordResult);
            //}
            return Ok(borrowRecordResult);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> singleBorrowRecord(int id)
        {
            //check if current user has admin role he can check any records else check if the record is owned by the logged in user
            var borrowRecordResult = await borrowRecordService.GetBorrowRecord(id, User);
            if (!borrowRecordResult.IsSuccess)
            {
                if (borrowRecordResult.error == Errors.DoesntBelong) return Unauthorized(borrowRecordResult);
                return NotFound(borrowRecordResult);
            }
            return Ok(borrowRecordResult);
        }



    }
}