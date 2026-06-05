using System.ComponentModel.DataAnnotations;

namespace UdemyApi.DTOs
{
    public class InstructorPayoutDTO
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(18, MinimumLength = 9)]
        public string BankAccountNumber { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC Code format.")]
        public string IfscCode { get; set; } = string.Empty;
    }
}