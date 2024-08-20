using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductService.Models;
using ProductService.Service;
using ProductService.Service.IFolder;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProductService.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ICartService cartService;
        private readonly IProductService productService;
        private readonly ICouponService couponService;

        public ShoppingCartController(ICartService cartService, IProductService productService)
        {
            this.cartService = cartService;
            this.productService = productService;          
        }

        public async Task<IActionResult> ShoppingCartIndex()
        {
            ShoppingCartDto s = await LoadCartByUserIndex();
            return View(s);

        }

        public async Task<ShoppingCartDto> LoadCartByUserIndex()
        {
            ShoppingCartDto cart = new();
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            ResponseDto response = await cartService.GetCartAsync(userId);

            if (response.IsSuccess == true)
            {
                cart = JsonConvert.DeserializeObject<ShoppingCartDto>(Convert.ToString(response.Result));

                for (int i = 0; i < cart.CartDetails.Count(); i++)
                {
                    ResponseDto resp = await productService.GetProductByIdAsync(cart.CartDetails.ElementAt(i).ProductId);


                    cart.CartDetails.ElementAt(i).Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(resp.Result));
                }
                return cart;
            }
            else
            {
                TempData["carterror"] = response.Message;
            }
            return cart;
        }


        public async Task<IActionResult> AddToCart(int productId)
        {
            if (ModelState.IsValid)
            {
                ShoppingCartDto cartDto = new ShoppingCartDto()
                {
                    CartHeader = new CartHeaderDto
                    {
                        UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.First()?.Value
                    }
                };

                CartDetailsDto detailsDto = new CartDetailsDto()
                {
                    ProductCount = 1,
                    ProductId = productId,
                };

                List<CartDetailsDto> cartDetails = new() { detailsDto };
                cartDto.CartDetails = cartDetails;

                ResponseDto? resp = await cartService.Upsert(cartDto);
                if (resp.IsSuccess)
                {
                    return RedirectToAction(nameof(ShoppingCartIndex));
                }
                else
                {
                    TempData["carterror"] = resp.Message;
                }
            }

            return View();
        }

        [HttpPost]
        [ActionName("DeleteOneProductFromCart")]
        public async Task<IActionResult> DeleteOneProductFromCart(int cartDetailsId)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await cartService.DeleteOneProduct(cartDetailsId);

                if (response.IsSuccess == true) //questionable
                {
                    return RedirectToAction(nameof(ShoppingCartIndex));
                }
                else
                {
                    TempData["carterror"] = response.Message;
                }
            }
            return View();
        }


        [HttpPost]
        [ActionName("DeleteOneKindOfProductFromCart")]
        public async Task<IActionResult> DeleteOneKindOfProductFromCart(int cartDetailsId)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await cartService.DeleteOneKindOfProduct(cartDetailsId);

                if (response.IsSuccess == true) //questionable
                {
                    return RedirectToAction(nameof(ShoppingCartIndex));
                }
                else
                {
                    TempData["carterror"] = response.Message;
                }
            }
            return View();
        }


        [ActionName("RemoveCouponWeb")]
        public async Task<IActionResult> RemoveCouponWeb()
        {
            ShoppingCartDto s = await LoadCartByUserIndex();
            ResponseDto response = await cartService.RemoveCoupon(s.CartHeader.CouponCode, s.CartDetails.First().CartDetailsId);


            if (response.IsSuccess == true) //questionable
            {
                return RedirectToAction(nameof(ShoppingCartIndex));
            
            }
            else
            {
                TempData["carterror"] = response.Message;
            }
            return View();
        }

        [HttpPost("EmailCart")]
        public async Task<IActionResult> EmailCart()
        {
            ShoppingCartDto cart = await LoadCartByUserIndex();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            ResponseDto? response = await cartService.SendEmail(cart);
            if (response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly.";
                return RedirectToAction(nameof(ShoppingCartIndex));
            }
            return View();
        }

    }

}