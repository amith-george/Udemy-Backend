using System.ComponentModel.DataAnnotations;

namespace UdemyBackend.DTOs.Category
{
    public class SubcategoryCreateDto
    {
        [Required(ErrorMessage = "Subcategory name is required")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Parent Category ID is required")]
        public int CategoryId { get; set; }
    }
}