using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RewardService.API.Data;
using RewardService.API.Models;

namespace RewardService.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RewardController : ControllerBase
	{
		private readonly IMapper mapper;
		private readonly AppRewardDbContext dbContext;

		public RewardController(IMapper mapper, AppRewardDbContext dbContext)
        {
			this.mapper = mapper;
			this.dbContext = dbContext;
		}

		[HttpGet("GetRewardsByUserId/{userId}")]
		public ResponseDto GetRewardsByUserId(string userId)
		{
			ResponseDto resp = new();
			try
			{
				IEnumerable<Reward> rewards = dbContext.Rewards.Where(x => x.UserId == userId);
				resp.IsSuccess = true;
				resp.Result = rewards;
			}
			catch (Exception ex)
			{
				resp.IsSuccess = false;
				resp.Message = ex.Message.ToString();
			}

			return resp;
		}
    }
}
