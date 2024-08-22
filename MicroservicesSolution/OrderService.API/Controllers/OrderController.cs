using AutoMapper;
using Azure;
using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using OrderService.API.Data;
using OrderService.API.Models.Domain;
using OrderService.API.Models.DTO;
using OrderService.API.Service;
using OrderService.API.Service.IService;
using OrderService.API.Utility;
using Stripe;
using Stripe.Checkout;


namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController: ControllerBase
    {
        
        private readonly AppOrderDbContext dbContext;
        private  IProductService productService;
        private  IMapper mapper;
        private readonly IMessageBus messageBus;
        private readonly IConfiguration configuration;

        public OrderController(AppOrderDbContext dbContext,
            IProductService productService, IMapper mapper, 
            IMessageBus messageBus, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.productService = productService;
            this.mapper = mapper;
            this.messageBus = messageBus;
            this.configuration = configuration;
        }

       
        [HttpGet("GetOrders")]
        public ResponseDto? Get(string? userId = "")
        {
            ResponseDto resp = new();
            try
            {
                IEnumerable<OrderHeader> objList;
                if (User.IsInRole(SD.RoleAdmin.ToLower()))
                {
                    objList = dbContext.OrderHeaders.Include(u => u.OrderDetails).OrderByDescending(u => u.OrderHeaderId).ToList();
                }
                else
                {
                    objList = dbContext.OrderHeaders.Include(u => u.OrderDetails).Where(u => u.UserId == userId).OrderByDescending(u => u.OrderHeaderId).ToList();
                }
                resp.Result = mapper.Map<IEnumerable<OrderHeaderDto>>(objList);
                resp.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message.ToString();                
            }

            return resp;
        }


       
        [HttpGet("GetOrder/{id:int}")]
        public ResponseDto? Get(int id)
        {
            ResponseDto resp = new();
            try
            {
                OrderHeader orderHeader = dbContext.OrderHeaders.Include(
                    u => u.OrderDetails).First(u => u.OrderHeaderId == id);
                resp.Result = mapper.Map<OrderHeaderDto>(orderHeader);
				resp.IsSuccess = true;
			}
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message.ToString();
            }

            return resp;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus([FromRoute] int orderId, [FromBody] string newStatus)
        {
            ResponseDto resp = new();
            try
            {
                OrderHeader orderHeader = dbContext.OrderHeaders.First(x => x.OrderHeaderId == orderId);
                if(orderHeader != null)
                {
                    if(newStatus == SD.Status_Cancelled)
                    {
                        //give refund
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId
                        };

                        var service = new RefundService();
                        Refund refund = service.Create(options);
                        orderHeader.Status = newStatus;
                    }
                    orderHeader.Status = newStatus;
                    dbContext.SaveChanges();
					resp.IsSuccess = true;
				}
                else
                {
                    resp.IsSuccess = false;
                    resp.Message = "Not Found";
                }

            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message.ToString();
            }
            return resp;
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


        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            ResponseDto resp = new();
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment"
                   // Discounts = new List<SessionDiscountOptions>()
                };

               
                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.99 -> 2099
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.ProductCount
                    };

                    options.LineItems.Add(sessionLineItem);
                }
                if (stripeRequestDto.OrderHeader.Discount > 0 && !string.IsNullOrEmpty(stripeRequestDto.OrderHeader.CouponCode))
                {
                    options.Discounts = new List<SessionDiscountOptions>
            {
                new SessionDiscountOptions
                {
                    Coupon = stripeRequestDto.OrderHeader.CouponCode
                }
            };
                }
                var service = new SessionService();
                Session session = service.Create(options);
                stripeRequestDto.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = dbContext.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                dbContext.SaveChanges();
                resp.Result = stripeRequestDto;
                resp.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message.ToString();
                
            }

            return resp;
        }


        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            ResponseDto resp = new();
            try
            {
                OrderHeader orderHeader = dbContext.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);
                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);
                string emailMessage = "Your order details are below:\n";
                IEnumerable<OrderDetails> orderDetails = dbContext.OrderDetails.Where(x => x.OrderHeaderId == orderHeaderId);
                foreach (var item in orderDetails)
                {
                    emailMessage += "- " + item.productName + " (x" + item.ProductCount + ")\n";
                }
                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if(paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;
                    dbContext.SaveChanges();
                    RewardDto rewardDto = new()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal / 2), //will this cause problems??
                        UserId = orderHeader.UserId,
                        Email = orderHeader.Email,
                        OrderTotal = orderHeader.OrderTotal,
                        OrderTime = orderHeader.OrderTime,
                        EmailMessage = emailMessage
                    };

                    string topicName = configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await messageBus.publishMessage(rewardDto, topicName);



                    resp.Result = mapper.Map<OrderHeaderDto>(orderHeader);
                    resp.IsSuccess = true;
                }
                else
                {
                    resp.Message = "The status of the order is not approved";
                    resp.IsSuccess = false;
                }
                
            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message.ToString();

            }

            return resp;
        }
    }
   
}
