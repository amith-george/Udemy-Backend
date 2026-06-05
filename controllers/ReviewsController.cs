using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyBackend.Data;
using UdemyBackend.DTO;
using UdemyBackend.Models;
using System.Threading.Tasks;
using System;

namespace UdemyBackend.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context) {
            _context = context;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitReview([FromBody] ReviewDTO reviewDto) {
            // 1. Check if the user is actually enrolled in the course
            var isEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == reviewDto.StudentId && e.CourseId == reviewDto.CourseId);

            if (!isEnrolled) {
                return BadRequest("You must buy the course to leave a review.");
            }

            // 2. Map DTO to Model
            var review = new Review {
                CourseId = reviewDto.CourseId,
                UserId = reviewDto.StudentId, // The Review model has UserId, while DTO has StudentId
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            // 3. Save to database
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            
            return Ok("Review submitted successfully!");
        }
    }
}
