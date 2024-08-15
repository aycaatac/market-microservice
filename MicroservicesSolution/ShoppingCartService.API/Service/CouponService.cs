using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using ShoppingCartService.API.Models.DTO;
using ShoppingCartService.API.Service.IService;
using ShoppingCartService.Models.DTO;
using System.Net.Http;

namespace ShoppingCartService.API.Service
{
    public class CouponService:ICouponService
    {
        private readonly IHttpClientFactory clientFactory;

        public CouponService(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public Task<CouponDto> CreateCoupon(CouponDto coupon)
        {
            throw new NotImplementedException();
        }

        public async Task<CouponDto> GetCoupon(string couponCode)
        {
            var client = clientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
            var apiContet = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContet);
            if (resp != null && resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
            }
            return new CouponDto();
        }

    }
}
