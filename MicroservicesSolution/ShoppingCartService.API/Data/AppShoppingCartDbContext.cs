using Microsoft.EntityFrameworkCore;
using ShoppingCartService.API.Models.Domain;


namespace ShoppingCartService.API.Data
{
    public class AppShoppingCartDbContext : DbContext
    {
        public AppShoppingCartDbContext(DbContextOptions<AppShoppingCartDbContext> options) : base(options)
        {
        }

        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; } //bir tane mi cart details seti????? her birine bir tane set yok mu

        //seeding yapılmayacaksa onmodelcreating metodunun overridelanmasına gerek yok
/*protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

}*/

}
}