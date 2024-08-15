using Microsoft.AspNetCore.Identity;
using System.Security;

namespace AuthService.API.Models.Domain
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string? Role { get; set; }// = "defaultUser";
    }
}
