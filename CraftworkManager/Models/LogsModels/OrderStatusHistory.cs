using CraftworkManager.Models.LogsModels;

namespace CraftworkManager.Models.Logs
{
    public class OrderStatusHistory : StatusHistory
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public OrderStatus Status { get; set; }

    }
}
