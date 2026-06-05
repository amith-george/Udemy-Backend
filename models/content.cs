namespace UdemyBackend.Models
{
    public class Content
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string VideoUrl { get; set; }
    
        public int SectionId { get; set; }
        public int CourseId { get; set; }

        public Section Section { get; set; }
        public Course Course { get; set; }
    }

}
