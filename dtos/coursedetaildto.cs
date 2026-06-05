using System.Collections.Generic;
using UdemyBackend.DTOs.Section;

namespace UdemyBackend.DTOs.Course
{
    public class CourseDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Status { get; set; }
        public bool IsQuiz { get; set; }

        public int InstructorId { get; set; }
        public string InstructorName { get; set; }
        public string InstructorBio { get; set; } 

        public string SubcategoryName { get; set; }

        // The complete syllabus hierarchy
        public List<SectionWithContentsDto> Sections { get; set; } = new List<SectionWithContentsDto>();
    }
}