using Newtonsoft.Json.Linq;
using ProductService.Service.IFolder;
using ProductService.Utility;

namespace ProductService.Service
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor contextAccessor;

        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public void ClearToken()
        {
            contextAccessor.HttpContext?.Response.Cookies
                .Delete(SD.TokenCookie);
        }

        public string? GetToken()
        {
            string? token = null;
            bool? hasToken = contextAccessor.HttpContext?.Request
                .Cookies.TryGetValue(SD.TokenCookie, out token);
            return hasToken is true ? token : null;
        }

        public void SetToken(string token)
        {
            contextAccessor.HttpContext?.Response.Cookies
                .Append(SD.TokenCookie, token);
        }
    }
}
