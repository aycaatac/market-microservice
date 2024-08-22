using ProductService.Models;
using ProductService.Service.IFolder;
using ProductService.Utility;

namespace ProductService.Service
{
	public class RewardService : IRewardService
	{
		private readonly IBaseService baseService;

		public RewardService(IBaseService baseService)
        {
			this.baseService = baseService;
		}

        public async Task<ResponseDto?> GetRewards(string userId)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Url = SD.RewardApiBase + "/api/reward/GetRewardsByUserId/" + userId
			});
		}

		
	}
}
