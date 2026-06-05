namespace UdemyBackend.DTOs.Course
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Status { get; set; } 
        public bool IsQuiz { get; set; }
        
        // Flattened data for quick UI rendering
        public string InstructorName { get; set; } 
        public string SubcategoryName { get; set; } 
    }
}