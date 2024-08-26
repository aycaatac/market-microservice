using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductService.Models;
using ProductService.Service.IFolder;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace ProductService.Controllers
{
	public class RewardController : Controller
	{
		private readonly IRewardService rewardService;

		public RewardController(IRewardService rewardService)
        {
			this.rewardService = rewardService;
		}

        public async Task<IActionResult> RewardIndex()
		{
            //var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
			//var points = await GetTotalPointsByUserId(userId);
			//if(points > 1000 )
			//{
				
				//return RedirectToAction(nameof(RewardCouponGainIndex), new { userId });
            //}
			//else
			//{
                IEnumerable<RewardDto> rewards = await LoadRewards();
				
                return View(rewards);
            //}

		}

        public async Task<IActionResult> RewardIndexOrig()
        {
            IEnumerable<RewardDto> rewards = await LoadRewards();

            return View(rewards);

        }

        public async Task<IEnumerable<RewardDto>> LoadRewards()
		{
			var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
			ResponseDto resp = await rewardService.GetRewards(userId);
			
			if (resp.IsSuccess)
			{
				IEnumerable < RewardDto > rewards = JsonConvert.DeserializeObject<IEnumerable<RewardDto>>(Convert.ToString(resp.Result));
				return rewards;
			}

			return null;

		}

		public async Task<int> GetTotalPointsByUserId(string userId)
		{
            ResponseDto resp = await rewardService.GetRewards(userId);

            if (resp.IsSuccess)
            {
                IEnumerable<RewardDto> rewards = JsonConvert.DeserializeObject<IEnumerable<RewardDto>>(Convert.ToString(resp.Result));
				int totalPoints = 0;
                foreach (var reward in rewards)
                {
					totalPoints += reward.RewardsActivity;
                }

				return totalPoints;
            }

			return 0;
        }

		public async Task<IActionResult> RewardCouponGainIndex(string userId)
		{
			int totalPoints = await GetTotalPointsByUserId(userId);
		
			if(totalPoints > 1000)
			{
				return View(totalPoints);
			}

			return RedirectToAction(nameof(LoadRewards));
		}

	}
}
