using ProductService.Models;

namespace ProductService.Service.IFolder
{
    public interface ICouponService
    {
        Task<ResponseDto?> GetCouponByCodeAsync(string couponCode);
        Task<ResponseDto?> GetAllCouponsAsync();
        Task<ResponseDto?> GetAllRewardCouponsAsync();
        Task<ResponseDto?> GetCouponByIdAsync(int couponId);
        Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto);
        Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto);
        Task<ResponseDto?> DeleteCouponAsync(int couponId);
    }
}
