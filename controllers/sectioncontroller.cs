using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UdemyBackend.Data;
using UdemyBackend.DTOs.Section;
using UdemyBackend.Models;

namespace UdemyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/sections/course/5
        // Retrieves the syllabus for a specific course
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<SectionDto>>> GetSectionsForCourse(int courseId)
        {
            var sections = await _context.Sections
                .Where(s => s.CourseId == courseId)
                .OrderBy(s => s.SequenceOrder) // Crucial for displaying the syllabus correctly
                .Select(s => new SectionDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    SequenceOrder = s.SequenceOrder,
                    CourseId = s.CourseId
                })
                .ToListAsync();

            if (!sections.Any())
            {
                return NotFound(new { message = "No sections found for this course." });
            }

            return Ok(sections);
        }

        // GET: api/sections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SectionDto>> GetSection(int id)
        {
            var section = await _context.Sections
                .Where(s => s.Id == id)
                .Select(s => new SectionDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    SequenceOrder = s.SequenceOrder,
                    CourseId = s.CourseId
                })
                .FirstOrDefaultAsync();

            if (section == null) return NotFound(new { message = "Section not found." });

            return Ok(section);
        }

        // POST: api/sections
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SectionDto>> CreateSection([FromBody] SectionCreateDto dto)
        {
            var instructor = await GetAuthorizedInstructorAsync();
            if (instructor == null) return Forbid("Only registered instructors can create sections.");

            // SECURITY: Ensure the parent course exists AND belongs to this instructor
            var parentCourse = await _context.Courses.FindAsync(dto.CourseId);
            if (parentCourse == null) return NotFound(new { message = "Parent course not found." });
            
            if (parentCourse.InstructorId != instructor.Id)
            {
                return Forbid("You do not have permission to add sections to this course.");
            }

            var section = new Section
            {
                Title = dto.Title,
                SequenceOrder = dto.SequenceOrder,
                CourseId = dto.CourseId
            };

            _context.Sections.Add(section);
            await _context.SaveChangesAsync();

            var createdDto = new SectionDto
            {
                Id = section.Id,
                Title = section.Title,
                SequenceOrder = section.SequenceOrder,
                CourseId = section.CourseId
            };

            return CreatedAtAction(nameof(GetSection), new { id = section.Id }, createdDto);
        }

        // PUT: api/sections/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateSection(int id, [FromBody] SectionUpdateDto dto)
        {
            // Eager load the Course so we can check the InstructorId
            var section = await _context.Sections
                .Include(s => s.Course) 
                .FirstOrDefaultAsync(s => s.Id == id);

            if (section == null) return NotFound(new { message = "Section not found." });

            var instructor = await GetAuthorizedInstructorAsync();
            if (instructor == null) return Forbid("Only registered instructors can modify sections.");

            // SECURITY: Check ownership through the loaded parent course
            if (section.Course.InstructorId != instructor.Id)
            {
                return Forbid("You do not have permission to modify this section.");
            }

            section.Title = dto.Title;
            section.SequenceOrder = dto.SequenceOrder;

            _context.Entry(section).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/sections/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSection(int id)
        {
            var section = await _context.Sections
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (section == null) return NotFound(new { message = "Section not found." });

            var instructor = await GetAuthorizedInstructorAsync();
            if (instructor == null) return Forbid("Only registered instructors can delete sections.");

            if (section.Course.InstructorId != instructor.Id)
            {
                return Forbid("You do not have permission to delete this section.");
            }

            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --- PRIVATE HELPER METHOD ---

        private async Task<Instructor> GetAuthorizedInstructorAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return null;
            }

            return await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
        }
    }
}