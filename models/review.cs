namespace UdemyBackend.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int Rating { get; set; } 
        public string Feedback { get; set; }
        public DateTime PostedAt { get; set; }
        public Student Student { get; set; }
        public Course Course { get; set; }
    }

}
