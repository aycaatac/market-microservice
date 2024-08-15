﻿using System.ComponentModel.DataAnnotations;

namespace CouponService.API.Models.Domain
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }
        [Required]
        public string CouponCode { get; set; }
        [Required]
        public double DiscountAmount { get; set; }
        public double MinAmount { get; set; }
    }
}
