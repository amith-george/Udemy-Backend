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
using UdemyBackend.DTOs.Content;
using UdemyBackend.Models;

namespace UdemyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ContentsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/contents/section/5
        // Loads all the videos/resources for a specific module
        [HttpGet("section/{sectionId}")]
        public async Task<ActionResult<IEnumerable<ContentDto>>> GetContentsForSection(int sectionId)
        {
            var contents = await _context.Contents
                .Where(c => c.SectionId == sectionId)
                .Select(c => new ContentDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    FilePath = c.FilePath,
                    VideoUrl = c.VideoUrl,
                    SectionId = c.SectionId,
                    CourseId = c.CourseId
                })
                .ToListAsync();

            if (!contents.Any()) return NotFound(new { message = "No content found for this section." });

            return Ok(contents);
        }

        // GET: api/contents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContentDto>> GetContent(int id)
        {
            var content = await _context.Contents
                .Where(c => c.Id == id)
                .Select(c => new ContentDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    FilePath = c.FilePath,
                    VideoUrl = c.VideoUrl,
                    SectionId = c.SectionId,
                    CourseId = c.CourseId
                })
                .FirstOrDefaultAsync();

            if (content == null) return NotFound(new { message = "Content not found." });

            return Ok(content);
        }

        // POST: api/contents
        [HttpPost]
        [Authorize]
        [RequestSizeLimit(2147483648)] // Allows up to 2GB uploads (Requires web.config/Kestrel setup too)
        public async Task<ActionResult<ContentDto>> CreateContent([FromForm] ContentCreateDto dto)
        {
            var instructor = await GetAuthorizedInstructorAsync();
            if (instructor == null) return StatusCode(403, new { message = "Only registered instructors can add content." });

            // SECURITY: Verify the instructor owns the course this content is being added to
            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null || course.InstructorId != instructor.Id)
            {
                return StatusCode(403, new { message = "You do not have permission to add content to this course." });
            }

            // INTEGRITY: Verify the target section actually belongs to the target course
            var section = await _context.Sections.FindAsync(dto.SectionId);
            if (section == null || section.CourseId != dto.CourseId)
            {
                return BadRequest("The specified section does not belong to the specified course.");
            }

            // Process File Uploads
            string savedVideoUrl = dto.VideoUrl; // Default to external URL if provided
            if (dto.VideoUpload != null)
            {
                savedVideoUrl = await SaveFileAsync(dto.VideoUpload, "Videos");
            }

            string savedResourcePath = null;
            if (dto.ResourceFileUpload != null)
            {
                savedResourcePath = await SaveFileAsync(dto.ResourceFileUpload, "Resources");
            }

            var content = new Content
            {
                Title = dto.Title,
                Description = dto.Description,
                SectionId = dto.SectionId,
                CourseId = dto.CourseId,
                VideoUrl = savedVideoUrl ?? "",
                FilePath = savedResourcePath ?? ""
            };

            _context.Contents.Add(content);
            await _context.SaveChangesAsync();

            var createdDto = new ContentDto
            {
                Id = content.Id,
                Title = content.Title,
                Description = content.Description,
                FilePath = content.FilePath,
                VideoUrl = content.VideoUrl,
                SectionId = content.SectionId,
                CourseId = content.CourseId
            };

            return CreatedAtAction(nameof(GetContent), new { id = content.Id }, createdDto);
        }

        // PUT: api/contents/5
        [HttpPut("{id}")]
        [Authorize]
        [RequestSizeLimit(2147483648)]
        public async Task<IActionResult> UpdateContent(int id, [FromForm] ContentUpdateDto dto)
        {
            var content = await _context.Contents.FindAsync(id);
            if (content == null) return NotFound(new { message = "Content not found." });

            var instructor = await GetAuthorizedInstructorAsync();
            if (instructor == null) return StatusCode(403, new { message = "Only registered instructors can modify content." });

            // SECURITY: Verify ownership via the CourseId
            var course = await _context.Courses.FindAsync(content.CourseId);
            if (course.InstructorId != instructor.Id)
            {
                return StatusCode(403, new { message = "You do not have permission to modify this content." });
            }

            // Update basic text fields
            content.Title = dto.Title;
            content.Description = dto.Description;

            // Handle Video Replacement
            if (dto.VideoUpload != null)
            {
                DeleteFile(content.VideoUrl); // Clean up old video
                content.VideoUrl = await SaveFileAsync(dto.VideoUpload, "Videos");
            }
            else if (!string.IsNullOrEmpty(dto.VideoUrl))
            {
                // Instructor swapped a local video upload for an external YouTube/Vimeo link
                DeleteFile(content.VideoUrl); 
                content.VideoUrl = dto.VideoUrl;
            }

            // Handle Resource Replacement
            if (dto.ResourceFileUpload != null)
            {
                DeleteFile(content.FilePath); // Clean up old zip/pdf
                content.FilePath = await SaveFileAsync(dto.ResourceFileUpload, "Resources");
            }

            _context.Entry(content).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/contents/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteContent(int id)
        {
            var content = await _context.Contents.FindAsync(id);
            if (content == null) return NotFound(new { message = "Content not found." });

            var instructor = await GetAuthorizedInstructorAsync();
            if (instructor == null) return StatusCode(403, new { message = "Only registered instructors can delete content." });

            var course = await _context.Courses.FindAsync(content.CourseId);
            if (course.InstructorId != instructor.Id)
            {
                return StatusCode(403, new { message = "You do not have permission to delete this content." });
            }

            // Clean up the physical files from the hard drive
            DeleteFile(content.VideoUrl);
            DeleteFile(content.FilePath);

            _context.Contents.Remove(content);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --- PRIVATE HELPER METHODS ---

        private async Task<Instructor> GetAuthorizedInstructorAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId)) return null;
            return await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
        }

        private async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0) return null;

            // Routes to either Uploads/Videos or Uploads/Resources based on the parameter
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", subFolder);
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

            return $"/Uploads/{subFolder}/{uniqueFileName}";
        }

        private void DeleteFile(string relativeUrl)
        {
            // Only attempt to delete if the URL is a local file path (not a YouTube link)
            if (string.IsNullOrEmpty(relativeUrl) || !relativeUrl.StartsWith("/Uploads/")) return;

            // Example relativeUrl: "/Uploads/Videos/123-abc.mp4"
            // We split it to find the subfolder and filename correctly
            var segments = relativeUrl.TrimStart('/').Split('/');
            if (segments.Length >= 3)
            {
                var subFolder = segments[1];
                var fileName = segments[2];
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", subFolder, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }
    }
}