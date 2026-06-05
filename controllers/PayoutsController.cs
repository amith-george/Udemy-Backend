using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UdemyBackend.Data;
using UdemyApi.DTOs;
using UdemyBackend.Models;

namespace UdemyApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PayoutsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PayoutsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetPayoutHistory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
            if (instructor == null) return Unauthorized();
            var instructorId = instructor.Id;

            var history = await _context.PayoutHistories
                .Where(p => p.InstructorId == instructorId)
                .OrderByDescending(p => p.PayoutDate)
                .ToListAsync();

            return Ok(history);
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPayout([FromBody] InstructorPayoutDTO model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
            if (instructor == null) return Unauthorized();
            var instructorId = instructor.Id;

            if (model.Amount <= 0) 
                return BadRequest(new { message = "Payout amount must be greater than zero." });

            instructor.BankAccountNumber = model.BankAccountNumber;
            instructor.IfscCode = model.IfscCode;
            
            var payout = new PayoutHistory
            {
                InstructorId = instructorId,
                AmountPaid = model.Amount,
                Status = "Processed",
                PayoutDate = DateTime.UtcNow
            };

            _context.PayoutHistories.Add(payout);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Payout successfully completed and logged." });
        }
    }
}