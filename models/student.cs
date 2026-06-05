namespace UdemyBackend.Models
{
    public class Student
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    
        public string Biography { get; set; }
        public string WebsiteLink { get; set; }
        public string FacebookLink { get; set; }
        public string XLink { get; set; }
        public string InstagramLink { get; set; }
        public string LinkedInLink { get; set; }
        public string GitHubLink { get; set; }
        public string Subscription { get; set; }
        public bool IsPublic { get; set; }

        public User User { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Cart> CartItems { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Certificate> Certificates { get; set; }
    }

}
