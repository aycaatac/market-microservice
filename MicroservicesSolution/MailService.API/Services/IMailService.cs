using MailService.API.Models;

namespace MailService.API.Services
{
    public interface IMailService
    {
        Task MailCartAndLog(ShoppingCartDto cartDto);
    }
}
