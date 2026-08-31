namespace Emart.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public int AddressId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal SubTotal { get; set; }

        public decimal ShippingAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal GrandTotal { get; set; }

        public string OrderStatus { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }
    }
}