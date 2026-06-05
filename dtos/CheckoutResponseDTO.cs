namespace YourProject.DTOs
{
    public class CheckoutResponseDTO
    {
        public string RazorpayOrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string CurrencySymbol { get; set; } = "₹";
    }
}