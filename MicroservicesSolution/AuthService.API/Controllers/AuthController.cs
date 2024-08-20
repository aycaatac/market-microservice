using AuthService.API.Models.Domain;
using AuthService.API.Models.DTO;
using AuthService.API.Service.IService;
using MessageBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IMessageBus messageBus;
        private readonly IConfiguration configuration;

        public AuthController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration)
        {
            this.authService = authService;
            this.messageBus = messageBus;
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ResponseDto> Register([FromBody] RegisterUserDto registerUserDto)
        {
            ResponseDto resp = new();
            //password uymasa bile registered diyor!!!!!!!!!!!!!!!!!!!!
            var user = await authService.Register(registerUserDto);
            if(!string.IsNullOrEmpty(user))
            {
                resp.Message = user;
                resp.IsSuccess = false;
                return resp;
            }            
            resp.Message = "User was registered! Please login!";
            resp.IsSuccess = true;

            ResponseDto _response = new();
            try
            {
                await messageBus.publishMessage(registerUserDto.Email, configuration.GetValue<string>("TopicAndQueueNames:AycaMarketRegisterQueue"));
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();
            }
            
            return resp;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ResponseDto> Login([FromBody] LoginUserDto loginUserDto)
        {
            ResponseDto resp = new();
            var response = await authService.Login(loginUserDto);
            if(response == null)
            {                
                resp.Message = "Username or password is invalid! Please try again!";
                resp.IsSuccess = false;
                return resp;
            }
            resp.Result = response;
            resp.IsSuccess = true;
            return resp;
        }

        [HttpPost]
        [Route("AssignRole")]
        public async Task<ResponseDto> AssignRole([FromBody] RegisterUserDto userDto)
        {

            ResponseDto resp = new();
            var response = await authService.AssignRole(userDto.Email.ToLower(), userDto.Role.ToLower());
            if(response == false)
            {
                resp.Message = "The role was NOT assigned to the user!";
                resp.IsSuccess = false;
                return resp;
               
            }
            resp.Result = response;
            resp.IsSuccess = true;
            return resp;
        }
    }
}
