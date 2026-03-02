namespace CraftworkManager.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public Guid BaseProductId { get; set; }
        public Product BaseProduct { get; set; }

        public string? Description { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
        public Guid? OrderId { get; set; }
        public Order? Order { get; set; } = null!;
        public OrderItemStatus Status { get; set; }

    }

    public enum OrderItemStatus
    {
        Pending,
        InProduction,
        Finished,
        Cancelled,
        Paused
    }
}
