using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using OrderService.API.Data;
using OrderService.API.Models.Domain;
using OrderService.API.Models.DTO;
using OrderService.API.Service;


namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> logger;
        private readonly IMessageProducer messageProducer;
        private readonly AppOrderDbContext dbContext;
        private readonly IMapper mapper;

        public OrderController(ILogger<OrderController> logger, IMessageProducer messageProducer, AppOrderDbContext dbContext
            ,IMapper mapper)
        {
            this.logger = logger;
            this.messageProducer = messageProducer;
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(OrderDto orderDto)
        {
            //do we design the model state requirements or are they ready 
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }


            await dbContext.Orders.AddAsync(mapper.Map<Order>(orderDto));
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
