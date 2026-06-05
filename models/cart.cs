public class Cart
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime AddedAt { get; set; }
    public Student Student { get; set; }
    public Course Course { get; set; }
}