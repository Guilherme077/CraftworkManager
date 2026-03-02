namespace CraftworkManager.Models
{
    public class AddItemViewModel
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}
