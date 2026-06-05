namespace UdemyBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfilePicture { get; set; }
        public bool IsActive { get; set; }
        public string otp {get; set; }
        public DateTime? OtpExpiryTime { get; set; }
        public int Role { get; set; }
        public DateTime CreatedAt { get; set; }
 
        public Student StudentProfile { get; set; }
        public Instructor InstructorProfile { get; set; }
    }
 
}
