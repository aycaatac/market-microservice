using ProductService.Models;
using ProductService.Service.IFolder;
using ProductService.Utility;

namespace ProductService.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService baseService;

        public AuthService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegisterUserDto registerUserDto)
        {

            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registerUserDto,
                Url = SD.AuthApiBase + "/api/auth/assignrole"
            });
        }

        public async Task<ResponseDto?> LoginAsync(LoginUserDto loginUserDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginUserDto,
                Url = SD.AuthApiBase + "/api/auth/login"
            });
        }

        public async Task<ResponseDto?> RegisterAsync(RegisterUserDto registerUserDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registerUserDto,
                Url = SD.AuthApiBase + "/api/auth/register"
            });
        }
    }
}
