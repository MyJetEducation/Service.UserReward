using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.UserReward.Models;
using Service.UserReward.Services;

namespace Service.UserReward.Jobs
{
	public abstract class NotificatorBase
	{
		protected readonly IDtoRepository DtoRepository;
		private readonly ITotalRewardService _totalRewardService;
		protected readonly ILogger Logger;

		protected NotificatorBase(IDtoRepository dtoRepository, ITotalRewardService totalRewardService, ILogger logger)
		{
			DtoRepository = dtoRepository;
			_totalRewardService = totalRewardService;
			Logger = logger;
		}

		protected async ValueTask Process(string userId, Action<StatusInfo, AchievementInfo> action)
		{
			Logger.LogDebug("ServiceBus Notificator {notificator} handled message for {user}", GetType().Name, userId);

			(StatusInfo statuses, AchievementInfo achievements) = await DtoRepository.GetAll(userId);

			action.Invoke(statuses, achievements);

			await _totalRewardService.CheckTotal(userId, statuses, achievements);
		}
	}
}