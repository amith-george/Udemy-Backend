using System.ComponentModel.DataAnnotations;

namespace UdemyBackend.DTOs.Category
{
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
    }
}