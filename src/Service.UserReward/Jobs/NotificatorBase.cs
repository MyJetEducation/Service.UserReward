using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.ServiceBus.Models;
using Service.UserReward.Helpers;
using Service.UserReward.Models;
using Service.UserReward.Services;

namespace Service.UserReward.Jobs
{
	public abstract class NotificatorBase
	{
		protected readonly IDtoRepository DtoRepository;
		private readonly ITotalRewardService _totalRewardService;
		protected readonly ILogger Logger;
		private readonly IServiceBusPublisher<UserRewardedServiceBusModel> _publisher;

		protected NotificatorBase(IDtoRepository dtoRepository, ITotalRewardService totalRewardService, ILogger logger, IServiceBusPublisher<UserRewardedServiceBusModel> publisher)
		{
			DtoRepository = dtoRepository;
			_totalRewardService = totalRewardService;
			Logger = logger;
			_publisher = publisher;
		}

		protected async ValueTask Process(string userId, Action<StatusInfo, AchievementInfo> action)
		{
			Logger.LogDebug("ServiceBus Notificator {notificator} handled message for {user}", GetType().Name, userId);

			(StatusInfo statuses, AchievementInfo achievements) = await DtoRepository.GetAll(userId);

			action.Invoke(statuses, achievements);

			await _totalRewardService.CheckTotal(userId, statuses, achievements);
			await PublishHelper.TryPublishUserRewarded(_publisher, userId, statuses, achievements);
		}
	}
}