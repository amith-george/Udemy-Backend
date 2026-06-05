namespace UdemyBackend.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int InstructorId { get; set; }
        public string Status { get; set; } // e.g., "Active", "Draft"
        public int Marks { get; set; }     // Total points possible

        public Course Course { get; set; }
        public Instructor Instructor { get; set; }
        public ICollection<Question> Questions { get; set; }
    }

}
