using Microsoft.EntityFrameworkCore;
using ProductService.API.Models.Domain;

namespace ProductService.API.Data
{
    public class AppProductDbContext : DbContext
    {
        public AppProductDbContext(DbContextOptions<AppProductDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 1,
                Name = "yogurt",
                Price = 49.99,
                Description = "productlarda yogurt urunu",
                CategoryName = "sut urunleri"
            },
            new Product
            {
                ProductId = 2,
                Name = "sut",
                Price = 19.99,
                Description = "productlarda sut urunu",
                CategoryName = "sut urunleri"
            }, new Product
            {
                ProductId = 3,
                Name = "ekmek",
                Price = 1.99,
                Description = "productlarda ekmek urunu",
                CategoryName = "hamur urunleri"
            },
            new Product
            {
                ProductId = 4,
                Name = "borek",
                Price = 39.99,
                Description = "productlarda borek urunu",
                CategoryName = "hamur urunleri"
            });
        }

    }
}