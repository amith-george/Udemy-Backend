namespace UdemyBackend.DTO {
    public class ProgressUpdateDTO {
        public int EnrollmentId { get; set; }
        public int SectionId { get; set; } 
        public decimal NewProgressPercentage {get; set; }
    }
}
