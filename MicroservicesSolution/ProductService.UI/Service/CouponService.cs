using ProductService.Models;
using ProductService.Service.IFolder;
using ProductService.Utility;

namespace ProductService.Service
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService baseService;

        public CouponService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = couponDto,
                Url = SD.CouponApiBase + "/api/coupon/create"
            });
        }

        public async Task<ResponseDto?> DeleteCouponAsync(int couponId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.CouponApiBase + "/api/coupon/deletebyid/" + couponId
            });
        }

        public async Task<ResponseDto?> GetAllCouponsAsync()
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponApiBase + "/api/coupon/getallresponse"
            });
        }

        public async Task<ResponseDto?> GetAllRewardCouponsAsync()
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponApiBase + "/api/coupon/GetAllRewardCoupons"
            });
        }

        public async Task<ResponseDto?> GetCouponByCodeAsync(string couponCode)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponApiBase + "/api/coupon/getbycode/" + couponCode
            });
        }

        public async Task<ResponseDto?> GetCouponByIdAsync(int couponId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponApiBase + "/api/coupon/getbyid/" + couponId
            });
        }

        public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = couponDto,
                Url = SD.CouponApiBase + "/api/coupon/update/" + couponDto.CouponId
            });
        }
    }
}
