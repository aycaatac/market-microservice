using Microsoft.EntityFrameworkCore;
using OrderService.API.Models.Domain;

namespace OrderService.API.Data
{
    public class AppOrderDbContext : DbContext
    {
        public AppOrderDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}
