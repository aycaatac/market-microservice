using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductService.Models;
using ProductService.Service;
using ProductService.Service.IFolder;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

namespace ProductService.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService productService;
        private readonly ICartService cartService;
        private readonly IEmailSenderService emailSenderService;

        public ProductController(IProductService productService, ICartService cartService,
            IEmailSenderService emailSenderService)
        {
            this.productService = productService;
            this.cartService = cartService;
            this.emailSenderService = emailSenderService;
        }

        public async Task<IActionResult> ProductIndex()
        {
           
            List<ProductDto> products = new();
            
            ResponseDto response = await productService.GetAllPorductsAsync();

            if(response != null && response.IsSuccess == true)
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["producterror"] = response.Message;
            }
            return View(products);
        }

        [Authorize(Roles = "admin,Admin,ADMIN,admın")]
        public async Task<IActionResult> ProductCreate()
        {            
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin,Admin,ADMIN,admın")]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await productService.CreateProductAsync(model);

                if (response.IsSuccess == true) //questionable
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["producterror"] = response.Message;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("ProductAddToCart")]
        public async Task<IActionResult> ProductAddToCart(int productId, string productName, double productPrice, string imageUrl)
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
                    return RedirectToAction("ShoppingCartIndex", "ShoppingCart");
                }
                else
                {
                    TempData["producterror"] = resp.Message;
                }
            }

            return View();
        }


        [Authorize(Roles = "admin,Admin,ADMIN,admın")]
        public async Task<IActionResult> ProductDelete(int productId)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await productService.DeleteProductAsync(productId);

                if (response.IsSuccess == true) //questionable
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["producterror"] = response.Message;
                }
            }
            return View();
        }

        [Authorize(Roles = "admin,Admin,ADMIN,admın")]
        public async Task<IActionResult> ProductUpdate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await productService.UpdateProductAsync(model);

                if (response.IsSuccess == true) //questionable
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["producterror"] = response.Message;
                }
            }
            return View(model);
        }
    }
}
