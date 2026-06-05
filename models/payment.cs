namespace UdemyBackend.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public decimal Amount { get; set; }
        public string GatewayTransactionId { get; set; } 
        public string Status { get; set; } 
        public DateTime ProcessedAt { get; set; }
        public Student Student { get; set; }
        public Course Course { get; set; }
    }

}
