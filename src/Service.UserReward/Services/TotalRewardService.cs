using System.Threading.Tasks;
using Service.Core.Client.Models;
using Service.UserReward.Models;

namespace Service.UserReward.Services
{
	public class TotalRewardService : ITotalRewardService
	{
		private readonly IStatusRewardService _statusRewardService;
		private readonly IAchievementRewardService _achievementRewardService;
		private readonly IDtoRepository _dtoRepository;

		public TotalRewardService(IStatusRewardService statusRewardService, IAchievementRewardService achievementRewardService, IDtoRepository dtoRepository)
		{
			_statusRewardService = statusRewardService;
			_achievementRewardService = achievementRewardService;
			_dtoRepository = dtoRepository;
		}

		public async ValueTask<CommonGrpcResponse> CheckTotal(string userId, StatusInfo statuses, AchievementInfo achievements)
		{
			_achievementRewardService.CheckTotal(statuses, achievements);
			_statusRewardService.CheckTotal(statuses, achievements);

			if (statuses.Changed)
				_achievementRewardService.CheckAllStatusesAchievement(statuses, achievements);

			bool achievementSaved = !achievements.Changed || await _dtoRepository.SetAchievements(userId, achievements);
			bool statusesSaved = !statuses.Changed || await _dtoRepository.SetStatuses(userId, statuses);

			return CommonGrpcResponse.Result(achievementSaved && statusesSaved);
		}
	}
}