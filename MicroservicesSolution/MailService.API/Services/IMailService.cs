using MailService.API.Message;
using MailService.API.Models;

namespace MailService.API.Services
{
    public interface IMailService
    {
        Task MailCartAndLog(ShoppingCartDto cartDto);
        Task RegisterLog(string emailAddress);
        Task LogOrderPlaced(RewardMessage rewardMessage);
    }
}
