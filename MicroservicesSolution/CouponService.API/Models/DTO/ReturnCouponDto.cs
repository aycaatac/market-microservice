using System.ComponentModel.DataAnnotations;

namespace CouponService.API.Models.DTO
{
    public class ReturnCouponDto
    {
        
        public int CouponId { get; set; }
        
        public string CouponCode { get; set; }
      
        public double DiscountAmount { get; set; }
        public double MinAmount { get; set; }
    }
}
