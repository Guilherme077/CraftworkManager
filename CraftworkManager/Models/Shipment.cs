using Microsoft.AspNetCore.Identity;

namespace CraftworkManager.Models
{
    public class Shipment
    {
        public Guid Id { get; set; }

        public string OrderId { get; set; }
        public Order Order { get; set; }
        
        public DateTime ShippedOn { get; set; }
        public string? TrackingCode { get; set; }
        public string? TransportCompany { get; set; }
        public ShipmentStatus Status { get; set; }

    }

    public enum ShipmentStatus
    {
        Pending,
        Shipped,
        Delivered,
        Cancelled
    }
}
