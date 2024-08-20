using ProductService.Models;
using ProductService.Service.IFolder;
using ProductService.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


namespace ProductService.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        private readonly ITokenProvider tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            this.authService = authService;
            this.tokenProvider = tokenProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            LoginUserDto loginDto = new();
            return View(loginDto);
           
            
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto userDto)
        {
            ResponseDto resp = await authService.LoginAsync(userDto);
            TempData["success1"] = null;
            if (resp.IsSuccess)
            {
                ReturnLoginUserDto responseDto = 
                    JsonConvert.DeserializeObject<ReturnLoginUserDto>
                    (Convert.ToString(resp.Result));
                await SignInUser(responseDto);
                tokenProvider.SetToken(responseDto.JwtToken);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["success1"] = "Username or password is incorrect!";
                return View(userDto);
            }
            return View(userDto);
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin, Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer, Value=SD.RoleCustomer},
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            TempData["success1"] = null;
            ResponseDto resp = await authService.RegisterAsync(registerUserDto);
            ResponseDto assignedRole;

            if(resp != null && resp.IsSuccess)
            {
                if (string.IsNullOrEmpty(registerUserDto.Role))
                {
                    registerUserDto.Role = SD.RoleCustomer;
                }
                
                //would it do anything if it didnt have the role??? what????
                    assignedRole = await authService.AssignRoleAsync(registerUserDto);
                
                if(assignedRole.IsSuccess)
                {
                    
                    return RedirectToAction(nameof(Login));

                }
                else
                {
                    TempData["success1"] = assignedRole.Message;
                    return View(registerUserDto);
                }
            }
            else
            {
                TempData["success1"] = resp.Message;
                var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin, Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer, Value=SD.RoleCustomer},
            };
                ViewBag.RoleList = roleList;
                return View(registerUserDto);
            }
            return View(registerUserDto);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(ReturnLoginUserDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.JwtToken);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
        }

        [Route("Auth/AccessDenied")]
        public IActionResult AccessDenied(string returnUrl)
        {
            // Check if the ReturnUrl is empty or is pointing to the AccessDenied page itself
            if (string.IsNullOrEmpty(returnUrl) || returnUrl == Url.Action("AccessDenied", "Auth"))
            {
                ViewData["ReturnUrl"] = Url.Action("Index", "Home");
            }
            else
            {
                // Redirect to ProductIndex if ReturnUrl contains 'Product'
                if (returnUrl.Contains("Product"))
                {
                    ViewData["ReturnUrl"] = Url.Action("ProductIndex", "Product");
                }
                // Redirect to CouponIndex if ReturnUrl contains 'Coupon'
                else if (returnUrl.Contains("Coupon"))
                {
                    ViewData["ReturnUrl"] = Url.Action("CouponIndex", "Coupon");
                }
                else
                {
                    // Default to the home page or any other safe page if no match is found
                    ViewData["ReturnUrl"] = Url.Action("Index", "Home");
                }
            }

            return View();
        }


    }
}
