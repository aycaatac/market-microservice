using AutoMapper;
using CouponService.API.Data;
using CouponService.API.Models.Domain;
using CouponService.API.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CouponService.API.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;

        public CouponController(AppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        //[Authorize(Roles = "stajyer")]
        [Route("GetAll")]
        public ResponseDto GetAll() //IActionResult or object??
        {
            ResponseDto resp = new();
            try
            {
                IEnumerable<Coupon> coupons = dbContext.Coupons.ToList();
                resp.Result = mapper.Map<IEnumerable<ReturnCouponDto>>(coupons);
                resp.IsSuccess = true;
                return resp;
            }
            catch(Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message;
            }
            return resp;
        }

        [HttpGet]      
        [Route("GetAllRewardCoupons")]
        public ResponseDto GetAllRewardCoupons() 
        {
            ResponseDto resp = new();
            try
            {
                IEnumerable<RewardCoupon> coupons = dbContext.RewardCoupons.ToList();
                resp.Result = mapper.Map<IEnumerable<RewardCouponDto>>(coupons);
                resp.IsSuccess = true;
                return resp;
            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message;
            }
            return resp;
        }

        [HttpGet]
        [Route("GetAllResponse")]
        public ResponseDto GetAllResponse()
        {
            IEnumerable<Coupon> coupons = dbContext.Coupons.ToList<Coupon>(); //?
            ResponseDto resp = new()
            {
                IsSuccess = true,
                Result = mapper.Map<List<CouponDto>>(coupons)
            };
            return resp;
        }

        [HttpGet]
        [Route("GetById/{id:int}")]
        public ResponseDto GetById([FromRoute]int id) //IActionResult or object??
        {
            ResponseDto resp = new();
            try
            {
                Coupon coupon = dbContext.Coupons.FirstOrDefault(x => x.CouponId == id);
                if(coupon!= null)
                {
                    resp.Result = mapper.Map<ReturnCouponDto>(coupon);
                    resp.IsSuccess = true;
                    return resp;
                }
                
            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message;
            }
            return resp;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode([FromRoute] string code) //IActionResult or object??
        {
            ResponseDto resp = new();
            try
            {
                Coupon coupon = dbContext.Coupons.FirstOrDefault(x => x.CouponCode.ToLower() == code.ToLower());
                if (coupon != null)
                {
                    resp.Result = mapper.Map<ReturnCouponDto>(coupon);
                    resp.IsSuccess = true;
                    return resp;                    
                }

            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message;
            }
            return resp;
        }


        [HttpPost]
        [Route("Create")]
        public ResponseDto Create([FromBody] CouponDto inputCoupon)
        {
            ResponseDto resp = new();
            try
            {
                //idsi unique mi veya kendisi null mı diye bakmam gerekir mi
                var couponDomainModel = mapper.Map<Coupon>(inputCoupon);
                dbContext.Coupons.Add(couponDomainModel);
                dbContext.SaveChanges();

                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(inputCoupon.DiscountAmount * 100),
                    Name = inputCoupon.CouponCode,
                    Currency = "usd",
                    Id = inputCoupon.CouponCode,
                };
                var service = new Stripe.CouponService();
                service.Create(options);



                var returnDto = mapper.Map<ReturnCouponDto>(couponDomainModel);
                resp.Result = returnDto;
                resp.IsSuccess = true;
                return resp;
            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message;
            }
            return resp;
        }


        [HttpPut]
        [Route("Update/{id:int}")]
        public ResponseDto Update([FromRoute] int id, [FromBody] CouponDto updateDto)
        {
            ResponseDto resp = new();
            try
            {
                var coupon = dbContext.Coupons.FirstOrDefault(x => x.CouponId == id);
                if (coupon == null)
                {
                    resp.IsSuccess = false;
                    resp.Message = "Not Found";
                }
                else
                {
                    coupon.CouponCode = updateDto.CouponCode;
                    coupon.DiscountAmount = updateDto.DiscountAmount;
                    coupon.MinAmount = updateDto.MinAmount;
                    dbContext.SaveChanges();
                    resp.Result = mapper.Map<ReturnCouponDto>(coupon);
                    resp.IsSuccess = true;
                    return resp;
                }
            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message;
            }
            return resp;
        }

        [HttpDelete]
        [Route("DeleteById/{id:int}")]
        public ResponseDto Delete([FromRoute] int id)
        {
            ResponseDto resp = new();

            try
            {
                var coupon = dbContext.Coupons.FirstOrDefault(x => x.CouponId == id);
                if (coupon == null)
                {
                    resp.IsSuccess = false;
                    resp.Message = "Not Found";
                }
                else
                {
                    dbContext.Coupons.Remove(coupon);
                    dbContext.SaveChanges();
                    var service = new Stripe.CouponService();
                    service.Delete(coupon.CouponCode);

                    resp.Result = mapper.Map<ReturnCouponDto>(coupon);
                    resp.IsSuccess = true;
                    return resp;
                }
            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message;
            }
            return resp;
        }
    }
}
