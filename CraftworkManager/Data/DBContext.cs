using CraftworkManager.Models;
using CraftworkManager.Models.Finance;
using CraftworkManager.Models.Logs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CraftworkManager.Data
{
    public class DBContext : IdentityDbContext<IdentityUser>
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistory { get; set; }
        public DbSet<ShipmentStatusHistory> ShipmentStatusHistory { get; set; }
    }
}
