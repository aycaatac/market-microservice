namespace AuthService.API.Models.DTO
{
    public class ReturnLoginUserDto
    {
        public UserDto User { get; set; }
        public string JwtToken { get; set; }
    }
}
