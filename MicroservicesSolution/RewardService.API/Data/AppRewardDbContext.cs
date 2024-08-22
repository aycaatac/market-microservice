using Microsoft.EntityFrameworkCore;
using RewardService.API.Models;



namespace RewardService.API.Data
{
    public class AppRewardDbContext : DbContext
    {
        public AppRewardDbContext(DbContextOptions<AppRewardDbContext> options) : base(options)
        {
        }

        public DbSet<Reward> Rewards { get; set; }        

    }
}