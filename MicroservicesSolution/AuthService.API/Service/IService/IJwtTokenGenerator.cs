using AuthService.API.Models.Domain;

namespace AuthService.API.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
