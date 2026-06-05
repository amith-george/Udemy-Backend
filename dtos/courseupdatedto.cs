using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UdemyBackend.DTOs.Course
{
    public class CourseUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public int SubcategoryId { get; set; }

        public bool IsQuiz { get; set; }

        // Optional: Only provided if the instructor is replacing the existing image
        public IFormFile ThumbnailImage { get; set; }
    }
}