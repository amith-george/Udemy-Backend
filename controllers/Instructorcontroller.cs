using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyBackend.Data;
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
            var instructor = await _context.Instructors.Include(i => i.User).FirstOrDefaultAsync(i => i.UserId == userId);
            if (instructor == null) return NotFound(new { message = "Instructor entry context missing." });

            instructor.Biography = updateDto.Biography;
            if (instructor.User != null) {
                instructor.User.ProfilePicture = updateDto.ProfilePictureUrl;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Instructor bio details updated successfully." });
        }
    }
}