using System.ComponentModel.DataAnnotations;

namespace UdemyBackend.DTOs.Section
{
    public class SectionUpdateDto
    {
        [Required(ErrorMessage = "Section title is required")]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        public int SequenceOrder { get; set; }
    }
}