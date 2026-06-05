using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyApi.Data;
using UdemyApi.Dtos;

namespace UdemyApi.Controllers
{
    [Authorize(Roles = "1")] // Strict role validation checking: Only allows Instructors (Role = 1)
    [ApiController]
    [Route("api/[controller]")]
    public class InstructorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InstructorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("profile/details")]
        public async Task<IActionResult> UpdateInstructorDetails([FromBody] UpdateProfileDto updateDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            // Fetch the specific instructor entity tied to this logged-in account
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
            if (instructor == null) return NotFound(new { message = "Instructor entry context missing." });

            // Update specific teacher resume data structures
            instructor.Headline = updateDto.Headline;
            instructor.Biography = updateDto.Biography;
            instructor.ProfilePictureUrl = updateDto.ProfilePictureUrl;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Instructor bio details updated successfully." });
        }
    }
}