namespace UdemyBackend.DTOs.Section
{
    public class SectionDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SequenceOrder { get; set; }
        public int CourseId { get; set; }
    }
}