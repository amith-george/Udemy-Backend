using System.Collections.Generic;

namespace UdemyBackend.DTO {
    public class QuizSubmissionDTO {
        public int QuizId { get; set; }
        public int EnrollmentId { get; set; }
        public List<QuestionAnswerDTO> Answers { get; set; } = new();
    }
    public class QuestionAnswerDTO {
        public int SelectedAnswerId { get; set; }
    }
}