public class Answer
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string AnswerText { get; set; }
    public bool IsCorrect { get; set; } // Essential for auto-grading MCQs

    public Question Question { get; set; }
}