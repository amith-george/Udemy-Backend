using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyBackend.Data;
using UdemyApi.Dtos;

namespace UdemyApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Extract the authenticated user ID claim from the JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var user = await _context.Users
                .Include(u => u.InstructorProfile) // Assuming navigation properties exist in models
                .Include(u => u.StudentProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound(new { message = "User profile not found." });

            var profileDto = new UserProfileDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                SystemRole = user.Role,
                CreatedAt = user.CreatedAt,
                InstructorDetails = user.InstructorProfile != null ? new InstructorProfileDto
                {
                    InstructorId = user.InstructorProfile.Id,
                    Headline = null,
                    Biography = user.InstructorProfile.Biography,
                    ProfilePictureUrl = user.ProfilePicture
                } : null,
                StudentDetails = user.StudentProfile != null ? new StudentProfileDto
                {
                    StudentId = user.StudentProfile.Id
                } : null
            };

            return Ok(profileDto);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            user.FullName = updateDto.FullName;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Core account name updated successfully." });
        }
    }
}