using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyBackend.Data;
using UdemyBackend.DTO;
using System.Linq;
using System.Threading.Tasks;

namespace UdemyBackend.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class QuizzesController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public QuizzesController(ApplicationDbContext context) {
            _context = context;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] QuizSubmissionDTO submission) {
            var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == submission.QuizId);

            if (quiz == null) return NotFound("Quiz not found.");
            
            int totalScore = 0;
            
            foreach (var studentAnswer in submission.Answers) {
                var dbAnswer = await _context.Answers.FindAsync(studentAnswer.SelectedAnswerId);
                    
                if (dbAnswer != null && dbAnswer.IsCorrect) {
                    var question = await _context.Questions.FindAsync(dbAnswer.QuestionId);
                    if (question != null) {
                        totalScore += question.Points; 
                    }
                }
            }

            var result = new QuizResultDTO {
                QuizId = quiz.Id,
                TotalScore = totalScore,
            };

            return Ok(result);
        }
    }
}
