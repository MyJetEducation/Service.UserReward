using System;
using System.Threading.Tasks;
using Service.Core.Client.Models;
using Service.UserReward.Models;

namespace Service.UserReward.Services
{
	public interface ITotalRewardService
	{
		ValueTask<CommonGrpcResponse> CheckTotal(Guid? userId, StatusInfo statuses, AchievementInfo achievements);
	}
}