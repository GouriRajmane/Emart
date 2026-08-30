namespace EMart.Models
{
    public class Payments
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public string? TransactionId { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}
