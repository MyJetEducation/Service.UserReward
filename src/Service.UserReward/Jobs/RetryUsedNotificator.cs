using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.Core.Domain.Models.Constants;
using Service.UserReward.Domain.Models;
using Service.UserReward.Grpc.ServiceBusModels;
using Service.UserReward.Helpers;

namespace Service.UserReward.Jobs
{
	public class RetryUsedNotificator
	{
		private readonly ILogger<RetryUsedNotificator> _logger;
		private readonly IDtoRepository _dtoRepository;
		private readonly ITotalRewardService _totalRewardService;

		public RetryUsedNotificator(ILogger<RetryUsedNotificator> logger,
			ISubscriber<IReadOnlyList<RetryUsedServiceBusModel>> subscriber,
			IDtoRepository dtoRepository,
			ITotalRewardService totalRewardService)
		{
			_logger = logger;
			_dtoRepository = dtoRepository;
			_totalRewardService = totalRewardService;
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<RetryUsedServiceBusModel> events)
		{
			foreach (RetryUsedServiceBusModel message in events)
			{
				Guid? userId = message.UserId;
				(List<StatusDto> statuses, List<UserAchievement> achievements) = await _dtoRepository.GetAll(userId);

				_logger.LogDebug("Handled {model} for {user}", nameof(RetryUsedServiceBusModel), userId);

				//за 10 использований сброса результатов
				bool achievementsChanged = achievements.SetAchievement(UserAchievement.BadLuck, () => message.TotalCount >= 10);

				await _totalRewardService.CheckTotal(userId, statuses, achievements, false, achievementsChanged);
			}
		}
	}
}