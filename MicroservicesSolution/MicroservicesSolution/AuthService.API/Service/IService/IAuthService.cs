using AuthService.API.Models.DTO;
using Microsoft.AspNetCore.Identity.Data;

namespace AuthService.API.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegisterUserDto registerUserDto);
        Task<ReturnLoginUserDto> Login(LoginUserDto loginUserDto);
        Task<bool> AssignRole(string email, string role);
    }
}
