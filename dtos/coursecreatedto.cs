using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Required for IFormFile

namespace UdemyBackend.DTOs.Course
{
    public class CourseCreateDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Price must be a valid number.")]
        public decimal Price { get; set; }

        [Required]
        public string Status { get; set; } // "Premium", "Free"

        [Required]
        public int SubcategoryId { get; set; }

        // Handles the actual image file upload from the frontend
        [Required(ErrorMessage = "A course thumbnail is required.")]
        public IFormFile ThumbnailImage { get; set; }
    }
}