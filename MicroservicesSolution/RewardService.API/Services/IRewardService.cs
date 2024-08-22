using RewardService.API.Message;
using RewardService.API.Models;

namespace RewardService.API.Services
{
    public interface IRewardService
    {
        Task<bool> UpdateRewards(RewardMessage rewardMessage);
    }
}
