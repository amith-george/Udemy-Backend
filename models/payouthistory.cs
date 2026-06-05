namespace UdemyBackend.Models
{
    public class PayoutHistory
    {
        public int Id { get; set; }
        public int InstructorId { get; set; }
        public decimal AmountPaid { get; set; }
        public string Status { get; set; } 
        public DateTime PayoutDate { get; set; }
        public Instructor Instructor { get; set; }
    }

}
