using System.Threading.Tasks;
using Service.Core.Client.Models;
using Service.UserReward.Models;

namespace Service.UserReward.Services
{
	public interface ITotalRewardService
	{
		ValueTask<CommonGrpcResponse> CheckTotal(string userId, StatusInfo statuses, AchievementInfo achievements);
	}
}