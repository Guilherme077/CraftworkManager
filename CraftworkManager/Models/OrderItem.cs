namespace CraftworkManager.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public string BaseProductId { get; set; } 
        public Product BaseProduct { get; set; }

        public string? Description { get; set; }
        public double? Cost { get; set; }
        public double? Price { get; set; }
        public int Quantity { get; set; }
        public string OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}
