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
        [Route("borrowrequest/{BookId}")]
        public async Task<IActionResult> RequestBook(int BookId, int borrowDuration)
        {
            if (borrowDuration <= 0) return BadRequest(new Error("Invalid Borrow Duration must be larger than 0"));
            var borrowRecordResult = await borrowRecordService.BorrowBook(BookId, User, borrowDuration);
            if (!borrowRecordResult.IsSuccess)
            {
                if (borrowRecordResult.error == Errors.DoesntExist(typeof(Book).Name)) return NotFound(borrowRecordResult);
                if (borrowRecordResult.error == Errors.NotAvailable) return BadRequest(borrowRecordResult);
            }
            return Ok(borrowRecordResult);
        }

        [HttpPut]
        [Authorize(Roles = "Member")]
        [Route("return/{BookId}")]
        public async Task<IActionResult> ReturnBook(int BookId)
        {
            string userId = User.FindFirst("Id")?.Value;
            var borrowRecordResult = await borrowRecordService.ReturnBook(BookId, userId);
            if (!borrowRecordResult.IsSuccess)
            {
                if (borrowRecordResult.error == Errors.DoesntExist(typeof(Book).Name)) return NotFound(borrowRecordResult);
                if (borrowRecordResult.error == Errors.NotAvailable) return BadRequest(borrowRecordResult);
            }
            return Ok(borrowRecordResult);
        }

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
                if (borrowRecordResult.error == Errors.DoesntExist(typeof(BorrowRecord).Name)) return NotFound(borrowRecordResult);
                return BadRequest(borrowRecordResult);
            }
            return Ok(borrowRecordResult);
        }


        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> editBorrowRecord(int id, string status)
        {
            //check if current user has admin role he can check any records else check if the record is owned by the logged in user
            var borrowRecordResult = await borrowRecordService.HandleBorrowRequests(id, status);
            if (!borrowRecordResult.IsSuccess)
            {
                if (borrowRecordResult.error == Errors.DoesntExist(typeof(BorrowRecord).Name)) return NotFound(borrowRecordResult);
                return BadRequest(borrowRecordResult);
            }
            return Ok(borrowRecordResult);
        }



    }
}