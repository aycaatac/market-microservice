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
	}
}
