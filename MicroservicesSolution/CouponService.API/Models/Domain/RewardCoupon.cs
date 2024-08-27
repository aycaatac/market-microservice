using System.ComponentModel.DataAnnotations;

namespace CouponService.API.Models.Domain
{
    public class RewardCoupon
    {
        [Key]
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public double DiscountAmount { get; set; }
        public double MinAmount { get; set; }


        public double RewardPointsNeeded { get; set; }
    }
}
