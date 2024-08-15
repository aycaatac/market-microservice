using AuthService.API.Data;
using AuthService.API.Models.Domain;
using AuthService.API.Models.DTO;
using AuthService.API.Service.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.API.Service
{
    public class AuthServiceImp : IAuthService
    {
        private readonly AppAuthDbContext appAuthDbContext;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IJwtTokenGenerator jwtTokenGenerator;

        public AuthServiceImp(AppAuthDbContext appAuthDbContext, 
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            this.appAuthDbContext = appAuthDbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<ReturnLoginUserDto> Login(LoginUserDto loginUserDto)
        {
           
            var user = await appAuthDbContext.Users.FirstOrDefaultAsync(x => x.Name.ToLower() == loginUserDto.Name.ToLower());
            bool isValid = await userManager.CheckPasswordAsync(user, loginUserDto.Password);
            if (user == null || !isValid)
            {
                return null;
            }
            else
            {

                //generate jwt token
                var token = jwtTokenGenerator.GenerateToken(user);

                UserDto userDto = new()
                {
                    Email = user.Email,
                    Id = user.Id,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber
                };

                ReturnLoginUserDto returnDto = new ReturnLoginUserDto()
                {
                    User = userDto,
                    JwtToken = token
                };

                return returnDto;
            }            
        }

        public async Task<string> Register(RegisterUserDto registerUserDto)
        {
            User user = new()
            {
                UserName = registerUserDto.Email,
                Email = registerUserDto.Email,
                NormalizedEmail = registerUserDto.Email.ToUpper(),
                PhoneNumber = registerUserDto.PhoneNumber,
                Name = registerUserDto.Name              
            };

            try
            {
                var result = await userManager.CreateAsync(user, registerUserDto.Password);

                if (result.Succeeded)
                {
                    var userToReturn = await appAuthDbContext.Users.FirstOrDefaultAsync(x => x.UserName == registerUserDto.Email);
                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        Id = userToReturn.Id,
                        PhoneNumber = userToReturn.PhoneNumber,
                        Name = userToReturn.Name                        
                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch(Exception ex)
            {
                return "Error encountered";
            }

            return "Error encountered";
        }

        async Task<bool> IAuthService.AssignRole(string email, string role)
        {
            var user = appAuthDbContext.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if(user != null)
            {
                if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                {
                    //turnes async into sync??
                    roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
                }
                await userManager.AddToRoleAsync(user, role);
                user.Role = role;
                await appAuthDbContext.SaveChangesAsync();
                return true;
            }           
            return false;
        }
    }
}
