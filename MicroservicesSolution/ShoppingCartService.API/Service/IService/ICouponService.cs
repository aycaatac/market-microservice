using ShoppingCartService.API.Models.DTO;

namespace ShoppingCartService.API.Service.IService
{
    public interface ICouponService
    {       
        Task<CouponDto> GetCoupon(string couponCode);
    }
}
