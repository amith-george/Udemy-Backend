using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UdemyBackend.Data;
using UdemyBackend.DTOs.Course;
using UdemyBackend.DTOs.Section;
using UdemyBackend.DTOs.Content;
using UdemyBackend.Models;

namespace UdemyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CoursesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/courses
        // Public endpoint for the catalog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Instructor).ThenInclude(i => i.User)
                .Include(c => c.Subcategory)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Price = c.Price,
                    ThumbnailUrl = c.ThumbnailUrl,
                    Status = c.Status,
                    IsQuiz = c.IsQuiz,
                    InstructorName = c.Instructor.User.FullName,
                    SubcategoryName = c.Subcategory.Name
                })
                .ToListAsync();

            return Ok(courses);
        }

        // GET: api/courses/5
        // Public endpoint for the course landing page
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDetailDto>> GetCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor).ThenInclude(i => i.User)
                .Include(c => c.Subcategory)
                .Include(c => c.Sections.OrderBy(s => s.SequenceOrder)) 
                    .ThenInclude(s => s.Contents)
                .Where(c => c.Id == id)
                .Select(c => new CourseDetailDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Price = c.Price,
                    ThumbnailUrl = c.ThumbnailUrl,
                    Status = c.Status,
                    IsQuiz = c.IsQuiz,
                    InstructorId = c.InstructorId,
                    InstructorName = c.Instructor.User.FullName,
                    InstructorBio = c.Instructor.Biography,
                    SubcategoryName = c.Subcategory.Name,
                    Sections = c.Sections.Select(s => new SectionWithContentsDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        SequenceOrder = s.SequenceOrder,
                        Contents = s.Contents.Select(ct => new ContentDto
                        {
                            Id = ct.Id,
                            Title = ct.Title,
                            Description = ct.Description,
                            FilePath = ct.FilePath,
                            VideoUrl = ct.VideoUrl,
                            SectionId = ct.SectionId,
                            CourseId = ct.CourseId
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (course == null) return NotFound(new { message = "Course not found." });

            return Ok(course);
        }

        // POST: api/courses
        [HttpPost]
        [Authorize] // Requires a valid JWT
        public async Task<ActionResult<CourseDto>> CreateCourse([FromForm] CourseCreateDto dto)
        {
            var instructor = await GetAuthorizedInstructorAsync();
            if (instructor == null) 
            {
                return Forbid("Only registered instructors can create courses.");
            }

            string savedFileUrl = await SaveThumbnailAsync(dto.ThumbnailImage);
            if (savedFileUrl == null) return BadRequest("Invalid or missing file upload.");

            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                Status = dto.Status,
                SubcategoryId = dto.SubcategoryId,
                ThumbnailUrl = savedFileUrl, 
                InstructorId = instructor.Id, // Secure assignment from JWT lookup
                IsQuiz = false 
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, new { message = "Course created successfully", courseId = course.Id });
        }

        // PUT: api/courses/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCourse(int id, [FromForm] CourseUpdateDto dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound(new { message = "Course not found." });

            var instructor = await GetAuthorizedInstructorAsync();
            if (instructor == null) return Forbid("Only registered instructors can modify courses.");

            // SECURITY: Ensure the person trying to update the course actually owns it
            if (course.InstructorId != instructor.Id)
            {
                return Forbid("You do not have permission to modify this course.");
            }

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.Price = dto.Price;
            course.Status = dto.Status;
            course.SubcategoryId = dto.SubcategoryId;
            course.IsQuiz = dto.IsQuiz;

            if (dto.ThumbnailImage != null)
            {
                DeleteThumbnail(course.ThumbnailUrl);
                course.ThumbnailUrl = await SaveThumbnailAsync(dto.ThumbnailImage);
            }

            _context.Entry(course).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/courses/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound(new { message = "Course not found." });

            var instructor = await GetAuthorizedInstructorAsync();
            if (instructor == null) return Forbid("Only registered instructors can delete courses.");

            // SECURITY: Ensure the person trying to delete the course actually owns it
            if (course.InstructorId != instructor.Id)
            {
                return Forbid("You do not have permission to delete this course.");
            }

            DeleteThumbnail(course.ThumbnailUrl);

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --- PRIVATE HELPER METHODS ---

        private async Task<Instructor> GetAuthorizedInstructorAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return null;
            }

            return await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
        }

        private async Task<string> SaveThumbnailAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Courses");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/Uploads/Courses/{uniqueFileName}";
        }

        private void DeleteThumbnail(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl)) return;

            var fileName = Path.GetFileName(relativeUrl);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Courses", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}