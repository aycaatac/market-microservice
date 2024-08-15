using ProductService.Models;

namespace ProductService.Service.IFolder
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginUserDto loginUserDto);
        Task<ResponseDto?> RegisterAsync(RegisterUserDto registerUserDto);
        Task<ResponseDto?> AssignRoleAsync(RegisterUserDto registerUserDto);
    }
}
