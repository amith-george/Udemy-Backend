using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyBackend.Data;
using UdemyApi.Dtos;
using UdemyBackend.Models; 
using UdemyApi.Services;
using System;
using System.Threading.Tasks;

namespace UdemyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;

        public AuthController(ApplicationDbContext context, IJwtService jwtService, IEmailService emailService)
        {
            _context = context;
            _jwtService = jwtService;
            _emailService = emailService;
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
            string otp = new Random().Next(100000, 999999).ToString();

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                Password = passwordHash,
                Role = registerDto.SystemRole,
                CreatedAt = DateTime.UtcNow,
                IsActive = false,
                otp = otp,
                ProfilePicture = "", // Use empty string instead of null to bypass the non-null DB constraint
                OtpExpiryTime = DateTime.UtcNow.AddMinutes(10)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Automatically create corresponding profile rows based on the selected role
            if (user.Role == 1) // Instructor
            {
                var instructorProfile = new Instructor { UserId = user.Id };
                _context.Instructors.Add(instructorProfile);
                await _context.SaveChangesAsync(); // Persist to generate the InstructorId
            }
            else if (user.Role == 2) // Student
            {
                var studentProfile = new Student { UserId = user.Id };
                _context.Students.Add(studentProfile);
                await _context.SaveChangesAsync();
            }

            // Send email
            string emailBody = $"<h1>Welcome to Udemy Clone!</h1><p>Your OTP for email verification is: <strong>{otp}</strong>. It will expire in 10 minutes.</p>";
            await _emailService.SendEmailAsync(user.Email, "Verify Your Email", emailBody);

            return Ok(new { message = "Registration successful. Please check your email for the OTP to verify your account." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            if (!user.IsActive)
            {
                return Unauthorized(new { message = "Email not verified. Please verify your email using the OTP sent to you." });
            }

            string? instructorIdStr = null;

            // If the logging-in user is an instructor, look up their profile to find their InstructorId
            if (user.Role == 1)
            {
                var instructorProfile = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == user.Id);
                if (instructorProfile != null)
                {
                    instructorIdStr = instructorProfile.Id.ToString();
                }
            }

            // Pass the details into your updated dynamic claims token builder
            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email, user.Role, instructorIdStr);

            return Ok(new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                SystemRole = user.Role
            });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return NotFound(new { message = "User not found." });

            if (user.IsActive) return BadRequest(new { message = "User is already verified." });

            if (user.otp != dto.Otp) return BadRequest(new { message = "Invalid OTP." });

            if (user.OtpExpiryTime < DateTime.UtcNow) return BadRequest(new { message = "OTP has expired. Please request a new one." });

            user.IsActive = true;
            user.otp = "";
            user.OtpExpiryTime = null;
            await _context.SaveChangesAsync();

            string? instructorIdStr = null;
            if (user.Role == 1)
            {
                var instructorProfile = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == user.Id);
                if (instructorProfile != null) instructorIdStr = instructorProfile.Id.ToString();
            }

            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email, user.Role, instructorIdStr);

            return Ok(new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                SystemRole = user.Role
            });
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return NotFound(new { message = "User not found." });

            if (user.IsActive) return BadRequest(new { message = "User is already verified." });

            string newOtp = new Random().Next(100000, 999999).ToString();
            user.otp = newOtp;
            user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(10);
            await _context.SaveChangesAsync();

            string emailBody = $"<h1>OTP Verification</h1><p>Your new OTP for email verification is: <strong>{newOtp}</strong>. It will expire in 10 minutes.</p>";
            await _emailService.SendEmailAsync(user.Email, "Verify Your Email", emailBody);

            return Ok(new { message = "A new OTP has been sent to your email." });
        }
    }
}