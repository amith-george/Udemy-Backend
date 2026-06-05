using System.ComponentModel.DataAnnotations;

namespace YourProject.DTOs
{
    public class CartItemRequestDTO
    {
        [Required]
        public int CourseId { get; set; }
    }
}