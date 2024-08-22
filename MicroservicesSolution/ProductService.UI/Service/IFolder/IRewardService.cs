using ProductService.Models;

namespace ProductService.Service.IFolder
{
	public interface IRewardService
	{
		Task<ResponseDto?> GetRewards(string userId);
	}
}
