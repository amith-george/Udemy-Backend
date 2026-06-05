public class Enrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrolledAt { get; set; }
    
    // Progress is tracked here per student
    public decimal ProgressPercentage { get; set; } 

    public Student Student { get; set; }
    public Course Course { get; set; }
}