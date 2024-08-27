using CouponService.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace CouponService.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<RewardCoupon> RewardCoupons { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RewardCoupon>().HasData(new RewardCoupon
            {
                CouponId = 1,
                CouponCode = "100FF",
                DiscountAmount = 10,
                MinAmount = 20,
                RewardPointsNeeded = 1000
            },
            new RewardCoupon
            {
                CouponId = 2,
                CouponCode = "200FF",
                DiscountAmount = 20,
                MinAmount = 40,
                RewardPointsNeeded = 2000
            },
            new RewardCoupon
            {
                CouponId = 3,
                CouponCode = "300FF",
                DiscountAmount = 30,
                MinAmount = 180,
                RewardPointsNeeded = 3500
            });
        }

    }
}