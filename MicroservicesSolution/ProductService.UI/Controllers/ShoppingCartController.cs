using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductService.Models;
using ProductService.Service;
using ProductService.Service.IFolder;
using ProductService.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProductService.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ICartService cartService;
        private readonly IProductService productService;
        private readonly IOrderService orderService;
        private readonly ICouponService couponService;

        public ShoppingCartController(ICartService cartService, 
            IProductService productService, IOrderService orderService)
        {
            this.cartService = cartService;
            this.productService = productService;
            this.orderService = orderService;
        }

        public async Task<IActionResult> ShoppingCartIndex()
        {
            ShoppingCartDto s = await LoadCartByUserIndex();
            return View(s);

        }

        public async Task<IActionResult> CheckOutIndex()
        {
            ShoppingCartDto s = await LoadCartByUserIndex();
            return View(s);
        }

        [HttpPost]
        [ActionName("CheckOutIndex")]
        public async Task<IActionResult> CheckOutIndex(ShoppingCartDto cartDto)
        {
            ShoppingCartDto cart = await LoadCartByUserIndex();
            cart.CartHeader.PhoneNumber = cartDto.CartHeader.PhoneNumber;
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Name = cartDto.CartHeader.Name;
            cart.CartHeader.Address = cartDto.CartHeader.Address;
            var response = await orderService.CreateOrder(cart);
            OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));

            if(response != null && response.IsSuccess)
            {

                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                StripeRequestDto stripeRequestDto = new()
                {
                    ApprovedUrl = domain + "ShoppingCart/Confirmation?orderId=" + orderHeaderDto.OrderHeaderId,
                    CancelUrl = domain + "ShoppingCart/CheckOutIndex",
                    OrderHeader = orderHeaderDto
                };

                var stripeResponse = await orderService.CreateStripeSession(stripeRequestDto);
                StripeRequestDto stripeResponseDto = JsonConvert.DeserializeObject<StripeRequestDto>(Convert.ToString(stripeResponse.Result));
                Response.Headers.Add("Location", stripeResponseDto.StripeSessionUrl);
                return new StatusCodeResult(303);
            }

            return View(cart);
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDto? response = await orderService.ValidateStripeSession(orderId);
            if (response.IsSuccess)
            {
                OrderHeaderDto orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
                if(orderHeader.Status == SD.Status_Approved)
                {
                    return View(orderId);
                }

            }
            //display the error if it is not approved mentioned in section 13 course 137
            return View(orderId);
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