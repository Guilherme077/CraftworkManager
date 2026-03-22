using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CraftworkManager.Models
{
    public class Shipment
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
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

        public string getAddressDescription()
        {
            return $"Envio para {City} - {State}, {Country}";
        }
        public string getDescription()
        {
            return $"{Order.getSmallDescription()} para {City}, {State}";

        }
        public decimal getTotalPrice()
        {
            decimal orderTotal = Order.getTotalPrice();
            if (ShippingCostIncludedOnPrice && ShippingCost.HasValue)
            {
                return orderTotal + ShippingCost.Value;
            }
            return orderTotal;
        }

    }

    public enum ShipmentStatus
    {
        [Display(Name = "Pendente")]
        Pending,
        [Display(Name = "A caminho")]
        Shipped,
        [Display(Name = "Entregue")]
        Delivered,
        [Display(Name = "Falhou")]
        Failed,
        [Display(Name = "Retornou")]
        Returned,
        [Display(Name = "Cancelado")]
        Cancelled
    }
}
