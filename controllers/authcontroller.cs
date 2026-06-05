using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyApi.Data;
using UdemyApi.Dtos;
using UdemyApi.Models; // Assuming your raw entities like User are here
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

            // Automatically create corresponding profile rows based on the selected role
            if (user.SystemRole == 1) // Instructor
            {
                var instructorProfile = new Instructor { UserId = user.UserId };
                _context.Instructors.Add(instructorProfile);
            }
            else if (user.SystemRole == 2) // Student
            {
                var studentProfile = new Student { UserId = user.UserId };
                _context.Students.Add(studentProfile);
            }

            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user.UserId.ToString(), user.Email, user.SystemRole);

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

            var token = _jwtService.GenerateToken(user.UserId.ToString(), user.Email, user.SystemRole);

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