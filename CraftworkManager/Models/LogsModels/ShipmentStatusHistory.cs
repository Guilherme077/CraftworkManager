using CraftworkManager.Models.LogsModels;

namespace CraftworkManager.Models.Logs
{
    public class ShipmentStatusHistory : StatusHistory
    {
        public Guid ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
        public ShipmentStatus Status { get; set; }
    }
}
