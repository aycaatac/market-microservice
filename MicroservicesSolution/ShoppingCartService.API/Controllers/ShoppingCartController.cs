using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartService.API.Data;
using ShoppingCartService.API.Models.Domain;
using ShoppingCartService.API.Models.DTO;
using ShoppingCartService.API.Service.IService;
using ShoppingCartService.Models.DTO;
using System.Reflection.PortableExecutable;

namespace ShoppingCartService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly AppShoppingCartDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IProductService productService;
        private readonly ICouponService couponService;

        public ShoppingCartController(AppShoppingCartDbContext appShoppingCartDbContext,
            IMapper mapper, IProductService productService, ICouponService couponService)
        {
            this.dbContext = appShoppingCartDbContext;
            this.mapper = mapper;
            this.productService = productService;
            this.couponService = couponService;
        }

        [HttpGet("GetShoppingCart/{userId}")]
        public async Task<ResponseDto> GetCart([FromRoute] string userId)
        {
            ResponseDto resp = new();
            try
            {
                var cartHeader = await dbContext.CartHeaders.FirstOrDefaultAsync(
                    x => x.UserId == userId);

                if (cartHeader == null)
                {
                    resp.IsSuccess = false;
                    resp.Message = "Not Found";
                    return resp;
                }

                ShoppingCartDto returnDto = new()
                {
                    CartHeader = mapper.Map<CartHeaderDto>(cartHeader)
                };

                returnDto.CartDetails = mapper.Map<IEnumerable<CartDetailsDto>>(dbContext.CartDetails.Where(x => x.CartHeaderId == returnDto.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> productDtos = await productService.GetProducts(); //bizim productlar orda olusuyo mu ki yoksa ayrı yerden mi çekiyo 

                //calculate the card total
                double cartTotal = 0;
                foreach (var product in returnDto.CartDetails)
                {
                    ProductDto p = productDtos.FirstOrDefault(x => x.ProductId == product.ProductId);

                    cartTotal += product.ProductCount * p.Price;
                }
                if (!string.IsNullOrEmpty(returnDto.CartHeader.CouponCode))
                {
                    CouponDto coupon = await couponService.GetCoupon(cartHeader.CouponCode);

                    if(coupon!= null)
                    {
                        if (cartTotal >= coupon.MinAmount)
                        {
                            cartTotal -= coupon.DiscountAmount;
                        }
                    }                    
                    returnDto.CartHeader.Discount = coupon.DiscountAmount;
                }
                returnDto.CartHeader.CartTotal = cartTotal;
                resp.IsSuccess = true;
                resp.Result = returnDto;
                return resp;

            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message.ToString();
                return resp;
            }
            resp.IsSuccess = false;
            resp.Message = "Not Found";
            return resp;
        }


        [HttpPost("ApplyCoupon/{couponCode}/{cartDetailsID}")]
        public async Task<ResponseDto> ApplyCoupon([FromRoute] string couponCode, [FromRoute] int cartDetailsID)
        {
            ResponseDto resp = new();
           
            CouponDto couponFound = await couponService.GetCoupon(couponCode);

            

            if (couponFound == null)
            {
                resp.IsSuccess = false;
                resp.Message = "Not Found";
                return resp;
            }
            else
            {
                //if coupon exists

                //check if cart details exist

                var existingDetails = await dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(x => x.CartDetailsId == cartDetailsID);

                if (existingDetails == null)
                {
                    resp.IsSuccess = false;
                    resp.Message = "Not Found";
                    return resp;
                }

                else
                {
                    //there is cart details and coupon
                    //find the cart header and make the discount

                    var existingHeader = await dbContext.CartHeaders.FirstOrDefaultAsync(x => x.CartHeaderId == existingDetails.CartHeaderId);
                    if (existingHeader == null)
                    {
                        resp.IsSuccess = false;
                        resp.Message = "Not Found";
                        return resp;
                    }

                    existingHeader.CouponCode = couponFound.CouponCode;
                    existingHeader.Discount += couponFound.DiscountAmount;
                    await dbContext.SaveChangesAsync();

                }
            }
            resp.IsSuccess = true;
            resp.Message = "Coupon has been applied!";
            return resp;

        }





        [HttpPost("RemoveCoupon/{couponCode}/{cartDetailsID}")]
        public async Task<ResponseDto> RemoveCoupon([FromRoute] string couponCode, [FromRoute] int cartDetailsID)
        {
            ResponseDto resp = new();

        

            CouponDto couponFound = await couponService.GetCoupon(couponCode);



            if (couponFound == null)
            {
                resp.IsSuccess = false;
                resp.Message = "Not Found";
                return resp;
            }            
            else
            {
                //if coupon exists

                //check if cart details exist

                var existingDetails = await dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(x => x.CartDetailsId == cartDetailsID);

                if (existingDetails == null)
                {
                    resp.IsSuccess = false;
                    resp.Message = "Not Found";
                    return resp;
                }

                else
                {
                    //there is cart details and coupon
                    //find the cart header and make the discount

                    var existingHeader = await dbContext.CartHeaders.FirstOrDefaultAsync(x => x.CartHeaderId == existingDetails.CartHeaderId);
                    if (existingHeader == null)
                    {
                        resp.IsSuccess = false;
                        resp.Message = "Not Found";
                        return resp;
                    }

                    existingHeader.CouponCode = "";
                    existingHeader.Discount -= couponFound.DiscountAmount;
                    await dbContext.SaveChangesAsync();

                }
            }
            resp.IsSuccess = true;
            resp.Message = "Coupon has been removed!";
            return resp;

        }

        //same method for insert and update
        //why is this async and not the others?


        //initial create, adding new items to the cart, updating quantities of existing items in the cart
        [HttpPost]
        [Route("CartUpsert")] //take id??
        public async Task<ResponseDto> Upsert(ShoppingCartDto shoppingCartDto)
        {
            ResponseDto resp = new();
            //check if there is an existing product with the product id
            IEnumerable<ProductDto> productDtos = await productService.GetProducts(); //bizim productlar orda olusuyo mu ki yoksa ayrı yerden mi çekiyo 

            bool exists = false;
            foreach (var product in productDtos)
            {
                if (product.ProductId == shoppingCartDto.CartDetails.First().ProductId)
                {
                    exists = true;
                }

            }

            if (!exists)
            {
                resp.IsSuccess = false;
                resp.Message = "Not Found";
                return resp;
            }

            else
            {
                try
                {
                    //is there an existing cart for this user
                    var existingHeader = await dbContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == shoppingCartDto.CartHeader.UserId);

                    if (existingHeader == null)
                    {
                        //create header AND details

                        //map the input dto into a domain model for header, already has id
                        CartHeader newHeader = mapper.Map<CartHeader>(shoppingCartDto.CartHeader);
                        await dbContext.CartHeaders.AddAsync(newHeader);
                        await dbContext.SaveChangesAsync();

                        //create cart details object
                        shoppingCartDto.CartDetails.First().CartHeaderId = newHeader.CartHeaderId;//bunu yapmasak?
                        await dbContext.CartDetails.AddAsync(mapper.Map<CartDetails>(shoppingCartDto.CartDetails.First()));
                        await dbContext.SaveChangesAsync();

                    }
                    else
                    {
                        //header var
                        //headerda bu producttan cart var mı
                        var existingDetails = await dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(x =>
                        x.ProductId == shoppingCartDto.CartDetails.First().ProductId && x.CartHeaderId ==
                        existingHeader.CartHeaderId);


                        if (existingDetails == null)
                        {
                            //product icin cart olusturulacak
                            shoppingCartDto.CartDetails.First().CartHeaderId = existingHeader.CartHeaderId;//bunu yapmasak?
                            await dbContext.CartDetails.AddAsync(mapper.Map<CartDetails>(shoppingCartDto.CartDetails.First()));
                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            //cartın product countu güncellenecek sadece 
                            shoppingCartDto.CartDetails.First().ProductCount += existingDetails.ProductCount;
                            shoppingCartDto.CartDetails.First().CartDetailsId = existingDetails.CartDetailsId;
                            shoppingCartDto.CartDetails.First().CartHeaderId = existingDetails.CartHeaderId;
                            dbContext.Update(mapper.Map<CartDetails>(shoppingCartDto.CartDetails.First()));
                            //ustteki satırda neyi updateledigimi nerden bilicek??
                            await dbContext.SaveChangesAsync();
                        }
                    }

                    resp.IsSuccess = true;
                    resp.Message = "Product is added to the cart!";
                    return resp;
                }
                catch (Exception ex)
                {
                    resp.IsSuccess = false;
                    resp.Message = ex.Message.ToString();
                    return resp;
                }
            }
        }

        [HttpDelete("DeleteOneProduct{cartDetailsId:int}")]
        //[Route("{id:int}")]
        public async Task<ResponseDto> DeleteOneProduct([FromRoute] int cartDetailsId)
        {
            ResponseDto resp = new();
            try
            {
                var cartDetails = await dbContext.CartDetails.FirstOrDefaultAsync(x => x.CartDetailsId == cartDetailsId);

                if (cartDetails == null)
                {
                    resp.IsSuccess = false;
                    resp.Message = "Not Found";
                    return resp;
                }


                int productCount = dbContext.CartDetails.Where(x => x.CartHeaderId == cartDetails.CartHeaderId).Count();
                if (productCount == 1)
                {
                    //one type of product but how many of them
                    if (cartDetails.ProductCount == 1)
                    {
                        //delete header and cart

                        var headerToRemove = await dbContext.CartHeaders.FirstOrDefaultAsync(x => x.CartHeaderId == cartDetails.CartHeaderId);

                        //do i need to remove the cart if i removed the header already
                        dbContext.CartHeaders.Remove(headerToRemove);
                        dbContext.CartDetails.Remove(cartDetails);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        //reduce the no of the same product
                        cartDetails.ProductCount = cartDetails.ProductCount - 1;
                        await dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    //dont delete header, do not know about the cart
                    if (cartDetails.ProductCount == 1)
                    {
                        //delete cart
                        dbContext.CartDetails.Remove(cartDetails);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        //reduce the no of the same product
                        cartDetails.ProductCount = cartDetails.ProductCount - 1;
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message.ToString();
                return resp;
            }
            resp.IsSuccess = true;
            resp.Message = "One product was deleted from the cart!";
            return resp;
        }



        [HttpDelete("DeleteOneKindOfProduct{id:int}")]
        //[Route("{id:int}")]
        public async Task<ResponseDto> DeleteOneKindOfProduct([FromRoute] int id)
        {
            ResponseDto resp = new();
            try
            {
                var cartDetails = await dbContext.CartDetails.FirstOrDefaultAsync(x => x.CartDetailsId == id);

                if (cartDetails == null)
                {
                    resp.IsSuccess = false;
                    resp.Message = "Not Found";
                    return resp;
                }


                int productCount = dbContext.CartDetails.Where(x => x.CartHeaderId == cartDetails.CartHeaderId).Count();
                dbContext.CartDetails.Remove(cartDetails);
                await dbContext.SaveChangesAsync();
                if (productCount == 1)
                {


                    //delete header and cart

                    var headerToRemove = await dbContext.CartHeaders.FirstOrDefaultAsync(x => x.CartHeaderId == cartDetails.CartHeaderId);

                    //do i need to remove the cart if i removed the header already
                    dbContext.CartHeaders.Remove(headerToRemove);
                    await dbContext.SaveChangesAsync();



                }

            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message.ToString();
                return resp;
            }
            resp.IsSuccess = true;
            resp.Message = "The product was deleted from the cart!";
            return resp;
        }
    }

}


