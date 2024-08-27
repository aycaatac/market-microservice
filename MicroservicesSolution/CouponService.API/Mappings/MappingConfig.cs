using AutoMapper;
using CouponService.API.Models.Domain;
using CouponService.API.Models.DTO;

namespace CouponService.API.Mappings
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>().ReverseMap();
                config.CreateMap<ReturnCouponDto, Coupon>().ReverseMap();
                config.CreateMap<RewardCoupon, RewardCouponDto>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
