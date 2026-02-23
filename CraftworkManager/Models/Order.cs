using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CraftworkManager.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public string userId { get; set; }
        public IdentityUser User { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdateOn { get; set; }
        public OrderStatus Status { get; set; }
        public string? Url { get; set; }
        public bool WithNF {  get; set; }
        public Payment PaymentWay { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }


    }

    public enum OrderStatus
    {
        [Display(Name = "Pendente")]
        Pending,
        [Display(Name = "Em Produção")]
        InProduction,
        [Display(Name = "Pronto para Envio")]
        ReadyToShip,
        [Display(Name = "Em Pausa")]
        Paused,
        [Display(Name = "Cancelado")]
        Cancelled,
        [Display(Name = "Enviado")]
        Shipped
    }
    public enum Payment
    {
        [Display(Name = "Dinheiro")]
        Cash,
        [Display(Name = "Pix")]
        Pix,
        [Display(Name = "Cartão de Crédito")]
        CreditCard,
        [Display(Name = "Cartão de Débito")]
        DebitCard,
        [Display(Name = "Cheque")]
        Check,
        [Display(Name = "Outro")]
        Other
    }
}
