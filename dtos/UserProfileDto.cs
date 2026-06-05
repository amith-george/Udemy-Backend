using System;

namespace UdemyApi.Dtos
{
    public class UserProfileDto
    {
        public int UserId { get; set; } // Use Guid if your DB relies on UUIDs
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int SystemRole { get; set; }
        public DateTime CreatedAt { get; set; }

        // Child object layers mapped from your student/instructor entities
        public StudentProfileDto? StudentDetails { get; set; }
        public InstructorProfileDto? InstructorDetails { get; set; }
    }

    public class StudentProfileDto
    {
        // Add specific student fields here if needed (e.g., Interests, CurrentEducation)
        public int StudentId { get; set; }
    }

    public class InstructorProfileDto
    {
        public int InstructorId { get; set; }
        public string? Headline { get; set; }
        public string? Biography { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}