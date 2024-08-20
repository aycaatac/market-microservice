using ProductService.Models;
using ProductService.Service.IFolder;
using ProductService.Utility;

namespace ProductService.Service
{
    public class CartService : ICartService
    {
        private readonly IBaseService baseService;

        public CartService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDto?> ApplyCouponAsync(ShoppingCartDto cartDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartApiBase + "/api/shoppingcart/ApplyCoupon"
            });
        }

        public async Task<ResponseDto?> DeleteOneKindOfProduct(int cartDetailsId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.ShoppingCartApiBase + "/api/shoppingcart/DeleteOneKindOfProduct/" + cartDetailsId
            });
        }

        public async Task<ResponseDto?> DeleteOneProduct(int cartDetailsId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.ShoppingCartApiBase + "/api/shoppingcart/DeleteOneProduct/" + cartDetailsId
            });
        }

        public async Task<ResponseDto?> GetCartAsync(string userId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ShoppingCartApiBase + "/api/shoppingcart/GetShoppingCart/" + userId
            });
        }

        public async Task<ResponseDto?> RemoveCoupon(string couponCode, int cartDetailsID)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.ShoppingCartApiBase + "/api/shoppingcart/RemoveCouponCont/" + couponCode + "/" + cartDetailsID
            });
        }

        public async Task<ResponseDto?> Upsert(ShoppingCartDto shoppingCartDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = shoppingCartDto,
                Url = SD.ShoppingCartApiBase + "/api/shoppingcart/CartUpsert"
            });
        }

        public async Task<ResponseDto?> SendEmail(ShoppingCartDto cartDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartApiBase + "/api/shoppingcart/EmailCartRequest"
            });
        }
    }
}
