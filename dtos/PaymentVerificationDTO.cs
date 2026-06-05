using System.ComponentModel.DataAnnotations;

namespace YourProject.DTOs
{
    public class PaymentVerificationDTO
    {
        [Required]
        public string RazorpayPaymentId { get; set; } = string.Empty;

        [Required]
        public string RazorpayOrderId { get; set; } = string.Empty;

        [Required]
        public string RazorpaySignature { get; set; } = string.Empty;
    }
}