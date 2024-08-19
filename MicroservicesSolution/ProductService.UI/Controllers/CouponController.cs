using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductService.Models;
using ProductService.Service;
using ProductService.Service.IFolder;

namespace ProductService.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService couponService;
        private readonly ICartService cartService;

        public CouponController(ICouponService couponService, ICartService cartService)
        {
            this.couponService = couponService;
            this.cartService = cartService;
        }

        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto> coupons = new();

            ResponseDto response = await couponService.GetAllCouponsAsync();

            if (response != null && response.IsSuccess == true)
            {
                coupons = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            return View(coupons);          
        }

        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await couponService.CreateCouponAsync(model);

                if (response.IsSuccess == true) //questionable
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("ApplyCouponToCart")]
        public async Task<IActionResult> ApplyCouponToCart(string couponCode)
        {
            if (ModelState.IsValid)
            {
                ShoppingCartDto cartDto = new ShoppingCartDto()
                {
                    CartHeader = new CartHeaderDto
                    {
                        UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.First()?.Value,
                        CouponCode = couponCode,
                    }
                };



                CartDetailsDto detailsDto = new CartDetailsDto()
                {
                    ProductCount = 1,
                    ProductId = 1,
                };

                List<CartDetailsDto> cartDetails = new() { detailsDto };
                cartDto.CartDetails = cartDetails;
                ResponseDto? resp = await cartService.ApplyCouponAsync(cartDto);
                if (resp.IsSuccess)
                {
                    return RedirectToAction("ShoppingCartIndex", "ShoppingCart");
                }

            }

            return View();
        }

        public async Task<IActionResult> CouponDelete(int couponId)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await couponService.DeleteCouponAsync(couponId);

                if (response.IsSuccess == true) //questionable
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
            }
            return View();
        }

        public async Task<IActionResult> CouponUpdate(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await couponService.UpdateCouponAsync(model);

                if (response.IsSuccess == true) //questionable
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
            }
            return View(model);
        }
    }
}

