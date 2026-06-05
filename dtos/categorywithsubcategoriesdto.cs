using System.Collections.Generic;

namespace UdemyBackend.DTOs.Category
{
    public class CategoryWithSubcategoriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        // Nested list of the flattened Subcategory DTOs
        public List<SubcategoryDto> Subcategories { get; set; } = new List<SubcategoryDto>();
    }
}