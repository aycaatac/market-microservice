
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RewardService.API.Data;
using RewardService.API.Message;
using RewardService.API.Models;
using System.Text;

namespace RewardService.API.Services
{
    public class RewardServiceImp : IRewardService
    {
        private DbContextOptions<AppRewardDbContext> dbOptions;

        public RewardServiceImp(DbContextOptions<AppRewardDbContext> options)
        {
            this.dbOptions = options;
        }

        public async Task<bool> UpdateRewards(RewardMessage rewardMessage)
        {
            try
            {
                Reward rewards = new()
                {
                    OrderId = rewardMessage.OrderId,
                    RewardsActivity = rewardMessage.RewardsActivity,
                    UserId = rewardMessage.UserId,
                    RewardsDate = DateTime.Now
                };
                await using var dbContext = new AppRewardDbContext(dbOptions);
                await dbContext.Rewards.AddAsync(rewards);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false; 
            }          
        }
    }
}
