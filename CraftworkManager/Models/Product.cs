using Microsoft.AspNetCore.Identity;

namespace CraftworkManager.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal StandardPrice { get; set; }
        public string Description { get; set; }
        public decimal StandardCost { get; set; }
        public string userId { get; set; }
        public IdentityUser User { get; set; }

    }
}
