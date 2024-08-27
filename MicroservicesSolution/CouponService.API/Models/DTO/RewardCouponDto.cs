namespace CouponService.API.Models.DTO
{
    public class RewardCouponDto
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public double DiscountAmount { get; set; }
        public double MinAmount { get; set; }


        public double RewardPointsNeeded { get; set; }
    }
}
