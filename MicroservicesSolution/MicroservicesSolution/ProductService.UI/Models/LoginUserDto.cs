using System.ComponentModel.DataAnnotations;

namespace ProductService.Models
{
    public class LoginUserDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
