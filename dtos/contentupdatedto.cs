using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UdemyBackend.DTOs.Content
{
    public class ContentUpdateDto
    {
        [Required(ErrorMessage = "Content title is required")]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        // Optional: Only provided if the instructor is replacing the existing video
        public IFormFile? VideoUpload { get; set; }

        public string VideoUrl { get; set; }

        // Optional: Only provided if replacing the existing downloadable resource
        public IFormFile? ResourceFileUpload { get; set; }
    }
}