using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UdemyApi.Data;
using UdemyApi.DTOs;
using UdemyApi.Models;

namespace UdemyApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PayoutsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PayoutsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetPayoutHistory()
        {
            var instructorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var history = await _context.PayoutHistories
                .Where(p => p.InstructorId == instructorId)
                .OrderByDescending(p => p.PayoutDate)
                .ToListAsync();

            return Ok(history);
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPayout([FromBody] InstructorPayoutDTO model)
        {
            var instructorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            if (model.Amount <= 0) 
                return BadRequest(new { message = "Payout amount must be greater than zero." });

            var payout = new PayoutHistory
            {
                InstructorId = instructorId,
                Amount = model.Amount,
                BankAccountNumber = model.BankAccountNumber,
                IfscCode = model.IfscCode,
                Status = "Processed",
                PayoutDate = DateTime.UtcNow
            };

            _context.PayoutHistories.Add(payout);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Payout successfully completed and logged." });
        }
    }
}