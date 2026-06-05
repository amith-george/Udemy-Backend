public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ThumbnailUrl { get; set; }
    public string Status { get; set; } // e.g., "Premium", "Free"
    public bool IsQuiz { get; set; }   // Indicates if a quiz exists
    
    public int InstructorId { get; set; }
    public int SubcategoryId { get; set; }

    public Instructor Instructor { get; set; }
    public Subcategory Subcategory { get; set; }
    public ICollection<Section> Sections { get; set; }
    public ICollection<Content> Contents { get; set; }
    public ICollection<Quiz> Quizzes { get; set; }
}