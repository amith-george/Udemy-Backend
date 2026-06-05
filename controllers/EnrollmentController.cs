using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyBackend.Data;
using UdemyBackend.DTO;
using UdemyBackend.Models;
using System.Threading.Tasks;
using System.Linq;

namespace UdemyBackend.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public EnrollmentsController(ApplicationDbContext context) {
            _context = context;
        }
        [HttpGet("student/{studentId}")]

        public async Task<IActionResult> GetStudentEnrollments(int studentId) {
            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course) 
                .ToListAsync();
            return Ok(enrollments);
        }
        [HttpPost("update-progress")]
        public async Task<IActionResult> UpdateProgress([FromBody] ProgressUpdateDTO dto) {
            var enrollment = await _context.Enrollments.FindAsync(dto.EnrollmentId);
            if (enrollment == null) return NotFound("Enrollment not found.");

            enrollment.ProgressPercentage = dto.NewProgressPercentage;

            if (enrollment.ProgressPercentage >= 100) {
                bool hasCert = await _context.Certificates.AnyAsync(c => c.StudentId == enrollment.StudentId && c.CourseId == enrollment.CourseId);
                if (!hasCert) {
                    var newCert = new Certificate { StudentId = enrollment.StudentId, CourseId = enrollment.CourseId, IssuedAt = DateTime.UtcNow, CertificateNumber = Guid.NewGuid().ToString(), DocumentUrl = "" };
                    _context.Certificates.Add(newCert);
                }
            }
            await _context.SaveChangesAsync();

            return Ok(enrollment);
        }
    }
    
    
    
}
