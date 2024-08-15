namespace ShoppingCartService.API.Models.DTO
{
    public class CouponDto
    {
        //do i need to return the coupon id
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public double DiscountAmount { get; set; }
        public double MinAmount { get; set; }
    }
}
