using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CraftworkManager.Models.Finance
{
    public class Transaction
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public bool Pending { get; set; }
        public Shipment? Shipment { get; set; }

        public decimal GetSignedAmount()
        {
            return Type == TransactionType.Income ? Amount : -Amount;
        }
    }

    public enum TransactionType
    {
        [Display(Name = "Receita")]
        Income,
        [Display(Name = "Despesa")]
        Expense
    }
}
