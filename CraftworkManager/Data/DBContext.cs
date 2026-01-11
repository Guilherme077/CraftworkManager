using CraftworkManager.Models;
using Microsoft.EntityFrameworkCore;

namespace CraftworkManager.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
    }
}
