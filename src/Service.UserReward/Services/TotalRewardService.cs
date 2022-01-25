using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Core.Domain.Models.Constants;
using Service.Core.Grpc.Models;
using Service.UserReward.Domain.Models;

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

		public async ValueTask<CommonGrpcResponse> CheckTotal(Guid? userId, List<StatusDto> statuses, List<UserAchievement> achievements, bool statusesChanged, bool achievementsChanged)
		{
			achievementsChanged = achievementsChanged || _achievementRewardService.CheckTotal(statuses, achievements);
			statusesChanged = statusesChanged || _statusRewardService.CheckTotal(statuses, achievements);

			if (statusesChanged)
				achievementsChanged = achievementsChanged || _achievementRewardService.CheckAllStatusesAchievement(statuses, achievements);

			bool achievementSaved = !achievementsChanged || await _dtoRepository.SetAchievements(userId, achievements);
			bool statusesSaved = !statusesChanged && await _dtoRepository.SetStatuses(userId, statuses);

			return CommonGrpcResponse.Result(achievementSaved && statusesSaved);
		}
	}
}