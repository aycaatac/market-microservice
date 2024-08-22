using ProductService.Models;
using ProductService.Service.IFolder;
using ProductService.Utility;
using System.Net.Http.Headers;

namespace ProductService.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService baseService;

        public OrderService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDto?> CreateOrder(ShoppingCartDto cartDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.OrderApiBase + "/api/order/createorder"
            });
        }

        public async Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = stripeRequestDto,
                Url = SD.OrderApiBase + "/api/order/CreateStripeSession"
            });
        }

        public async Task<ResponseDto?> GetAllOrder(string? userId)
        {	
			return await baseService.SendAsync(new RequestDto()
            {
				
				ApiType = SD.ApiType.GET,
                Data = userId,
                Url = SD.OrderApiBase + "/api/order/GetOrders?userId=" + userId,
                
            });
        }

        public async Task<ResponseDto?> GetOrder(int orderId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,               
                Url = SD.OrderApiBase + "/api/order/GetOrder/" + orderId
            });
        }

        public async Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = newStatus,
                Url = SD.OrderApiBase + "/api/order/UpdateOrderStatus/" + orderId
            });
        }

        public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = orderHeaderId,
                Url = SD.OrderApiBase + "/api/order/ValidateStripeSession"
            });
        }
    }
}

        

