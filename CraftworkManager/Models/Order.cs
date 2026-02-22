namespace CraftworkManager.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdateOn { get; set; }
        public OrderStatus Status { get; set; }
        public string? Url { get; set; }
        public bool WithNF {  get; set; }
        public Payment PaymentWay { get; set; }


    }

    public enum OrderStatus
    {
        Pending,
        InProduction,
        ReadyToShip,
        Paused,
        Cancelled,
        Shipped
    }
    public enum Payment
    {
        Cash,
        Pix,
        CreditCard,
        DebitCard,
        Check,
        Other
    }
}
