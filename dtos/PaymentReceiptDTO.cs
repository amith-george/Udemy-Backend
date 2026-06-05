namespace UdemyApi.DTOs
{
    public class PaymentReceiptDTO
    {
        public int PaymentId { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DatePaid { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}