namespace UdemyBackend.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    
        public string? Biography { get; set; }
        public string? WebsiteLink { get; set; }
        public string? FacebookLink { get; set; }
        public string? XLink { get; set; }
        public string? InstagramLink { get; set; }
        public string? LinkedInLink { get; set; }
        public string? GitHubLink { get; set; }
        public bool IsPublic { get; set; }

        public string? BankAccountNumber { get; set; }
        public string? IfscCode { get; set; }
        public string? Department { get; set; }

        public User User { get; set; }
        public ICollection<Course> Courses { get; set; }
        public ICollection<Quiz> Quizzes { get; set; }
        public ICollection<PayoutHistory> PayoutHistories { get; set; }
    }

}
