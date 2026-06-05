namespace UdemyBackend.DTOs.Content
{
    public class ContentDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        
        public string FilePath { get; set; }
        public string VideoUrl { get; set; }
        
        public int SectionId { get; set; }
        public int CourseId { get; set; }
    }
}