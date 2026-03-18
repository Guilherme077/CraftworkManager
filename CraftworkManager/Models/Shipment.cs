using Microsoft.AspNetCore.Identity;

namespace CraftworkManager.Models
{
    public class Shipment
    {
        public Guid Id { get; set; }

        public string OrderId { get; set; }
        public Order Order { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? ShippedOn { get; set; }
        public DateTime? DeliveredOn { get; set; }
        public string? TrackingCode { get; set; }
        public string? TrackingUrl { get; set; }
        public string? TransportCompany { get; set; }
        public ShipmentStatus Status { get; set; }
        public decimal? ShippingCost { get; set; }
        public bool ShippingCostIncludedOnPrice { get; set; } = false;
        public string? Notes { get; set; }
        
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string Country { get; set; } = "Brasil";

    }

    public enum ShipmentStatus
    {
        Pending,
        Shipped,
        Delivered,
        Failed,
        Returned,
        Cancelled
    }
}
