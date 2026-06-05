public class Section
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int SequenceOrder { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public ICollection<Content> Contents { get; set; }
}