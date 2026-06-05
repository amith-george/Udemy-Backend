using System.ComponentModel.DataAnnotations;

namespace UdemyApi.Dtos
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Name cannot be empty.")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        // Instructor fields (Optional; populated if the user is configuring an instructor profile)
        public string? Headline { get; set; }
        public string? Biography { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}