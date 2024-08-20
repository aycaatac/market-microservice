using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using OrderService.API.Data;
using OrderService.API.Models.Domain;
using OrderService.API.Models.DTO;
using OrderService.API.Service;
using OrderService.API.Service.IService;
using OrderService.API.Utility;


namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController: ControllerBase
    {
        
        private readonly AppOrderDbContext dbContext;
        private  IProductService productService;
        private  IMapper mapper;

        public OrderController(AppOrderDbContext dbContext,
            IProductService productService, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.productService = productService;
            this.mapper = mapper;
        }


        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] ShoppingCartDto cartDto)
        {
            ResponseDto response = new();
            try
            {
                OrderHeaderDto orderHeaderDto = mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderCreated = dbContext.OrderHeaders.Add(mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await dbContext.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                response.Result = orderHeaderDto;
                response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message.ToString();
                
            }
            return response;
        }
    }
   
}
