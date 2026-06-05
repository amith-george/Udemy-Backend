using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyApi.Data;
using UdemyApi.Dtos;
using UdemyApi.Models; 
using UdemyApi.Services;

namespace UdemyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest(new { message = "Email is already registered." });
            }

            // Securely hash password using BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                SystemRole = registerDto.SystemRole,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            string? instructorIdStr = null;

            // Automatically create corresponding profile rows based on the selected role
            if (user.SystemRole == 1) // Instructor
            {
                var instructorProfile = new Instructor { UserId = user.UserId };
                _context.Instructors.Add(instructorProfile);
                await _context.SaveChangesAsync(); // Persist to generate the InstructorId
                
                instructorIdStr = instructorProfile.InstructorId.ToString();
            }
            else if (user.SystemRole == 2) // Student
            {
                var studentProfile = new Student { UserId = user.UserId };
                _context.Students.Add(studentProfile);
                await _context.SaveChangesAsync();
            }

            // Generate JWT containing the custom InstructorId claim if applicable
            var token = _jwtService.GenerateToken(user.UserId.ToString(), user.Email, user.SystemRole, instructorIdStr);

            return Ok(new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                SystemRole = user.SystemRole
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            string? instructorIdStr = null;

            // If the logging-in user is an instructor, look up their profile to find their InstructorId
            if (user.SystemRole == 1)
            {
                var instructorProfile = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == user.UserId);
                if (instructorProfile != null)
                {
                    instructorIdStr = instructorProfile.InstructorId.ToString();
                }
            }

            // Pass the details into your updated dynamic claims token builder
            var token = _jwtService.GenerateToken(user.UserId.ToString(), user.Email, user.SystemRole, instructorIdStr);

            return Ok(new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                SystemRole = user.SystemRole
            });
        }
    }
}