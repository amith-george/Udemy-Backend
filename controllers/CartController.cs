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
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return Unauthorized();
            var studentId = student.Id;
            
            var items = await _context.Carts
                .Where(c => c.StudentId == studentId)
                .Include(c => c.Course)
                .Select(c => new { c.Id, c.CourseId, c.Course.Title, c.Course.Price })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemRequestDTO model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return Unauthorized();
            var studentId = student.Id;

            var courseExists = await _context.Courses.AnyAsync(c => c.Id == model.CourseId);
            if (!courseExists) return NotFound(new { message = "Course not found." });

            var alreadyInCart = await _context.Carts.AnyAsync(c => c.StudentId == studentId && c.CourseId == model.CourseId);
            if (alreadyInCart) return BadRequest(new { message = "Item already in cart." });

            var cartItem = new Cart
            {
                StudentId = studentId,
                CourseId = model.CourseId,
                AddedAt = DateTime.UtcNow
            };

            _context.Carts.Add(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course added to cart successfully." });
        }

        [HttpDelete("remove/{courseId}")]
        public async Task<IActionResult> RemoveFromCart(int courseId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return Unauthorized();
            var studentId = student.Id;

            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.StudentId == studentId && c.CourseId == courseId);
            if (cartItem == null) return NotFound(new { message = "Item not found in cart." });

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course removed from cart successfully." });
        }
    }
}