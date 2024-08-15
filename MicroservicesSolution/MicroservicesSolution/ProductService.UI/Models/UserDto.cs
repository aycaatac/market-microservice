namespace ProductService.Models
{
    public class UserDto
    {
        public string Id { get; set; } //neden guid returnlemiyoruz
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}
