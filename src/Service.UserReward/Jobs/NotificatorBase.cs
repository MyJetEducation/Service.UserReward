using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.UserReward.Models;
using Service.UserReward.Services;

namespace Service.UserReward.Jobs
{
	public abstract class NotificatorBase
	{
		private readonly IDtoRepository _dtoRepository;
		private readonly ITotalRewardService _totalRewardService;
		private readonly ILogger _logger;

		protected NotificatorBase(IDtoRepository dtoRepository, ITotalRewardService totalRewardService, ILogger logger)
		{
			_dtoRepository = dtoRepository;
			_totalRewardService = totalRewardService;
			_logger = logger;
		}

		protected async ValueTask Process(string userId, Action<StatusInfo, AchievementInfo> action)
		{
			_logger.LogDebug("ServiceBus Notificator {notificator} handled message for {user}", GetType().Name, userId);

			(StatusInfo statuses, AchievementInfo achievements) = await _dtoRepository.GetAll(userId);

			action.Invoke(statuses, achievements);

			await _totalRewardService.CheckTotal(userId, statuses, achievements);
		}
	}
}