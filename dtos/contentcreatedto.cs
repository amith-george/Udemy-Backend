using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Required to accept file uploads

namespace UdemyBackend.DTOs.Content
{
    public class ContentCreateDto
    {
        [Required(ErrorMessage = "Content title is required")]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Section ID is required")]
        public int SectionId { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        public int CourseId { get; set; }

        public IFormFile VideoUpload { get; set; }

        public string VideoUrl { get; set; }

        public IFormFile ResourceFileUpload { get; set; }
    }
}