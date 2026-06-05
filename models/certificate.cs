public class Certificate
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public string CertificateNumber { get; set; } 
    public string DocumentUrl { get; set; }
    public DateTime IssuedAt { get; set; }
    public Student Student { get; set; }
    public Course Course { get; set; }
}