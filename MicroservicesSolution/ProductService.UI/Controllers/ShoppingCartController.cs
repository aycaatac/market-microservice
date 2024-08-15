using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductService.Models;
using ProductService.Service.IFolder;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProductService.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ICartService cartService;

        public ShoppingCartController(ICartService cartService)
        {
            this.cartService = cartService;
        }

        public async Task<IActionResult> ShoppingCartIndex()
        {
            ShoppingCartDto cart = new();
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            ResponseDto response = await cartService.GetCartAsync(userId);

            if (response.IsSuccess == true)
            {
                cart = JsonConvert.DeserializeObject<ShoppingCartDto>(Convert.ToString(response.Result));
            }
            var cartDetails = cart.CartDetails;
            return View(cartDetails);
            
        }
    }
}
