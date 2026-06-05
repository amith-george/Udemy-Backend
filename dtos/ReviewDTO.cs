namespace UdemyBackend.DTO {
    public class ReviewDTO {
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public int Rating { get; set; } 
        public string Comment { get; set; }
    }
}
