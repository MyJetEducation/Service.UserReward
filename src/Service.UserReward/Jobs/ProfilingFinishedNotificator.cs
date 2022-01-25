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
	public class ProfilingFinishedNotificator
	{
		private readonly ILogger<ProfilingFinishedNotificator> _logger;
		private readonly IDtoRepository _dtoRepository;
		private readonly ITotalRewardService _totalRewardService;

		public ProfilingFinishedNotificator(ILogger<ProfilingFinishedNotificator> logger,
			ISubscriber<IReadOnlyList<ProfilingFinishedServiceBusModel>> subscriber,
			IDtoRepository dtoRepository,
			ITotalRewardService totalRewardService)
		{
			_logger = logger;
			_dtoRepository = dtoRepository;
			_totalRewardService = totalRewardService;
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<ProfilingFinishedServiceBusModel> events)
		{
			foreach (ProfilingFinishedServiceBusModel message in events)
			{
				Guid? userId = message.UserId;
				(List<StatusDto> statuses, List<UserAchievement> achievements) = await _dtoRepository.GetAll(userId);

				_logger.LogDebug("Handled {model} for {user}", nameof(ProfilingFinishedServiceBusModel), userId);

				//пройдено длинное профилирование
				bool achievementsChanged = statuses.SetStatus(UserStatus.MasterOfOpenness, () => message.Long);

				//закончил профилирование
				bool statusesChanged = achievements.SetAchievement(UserAchievement.Starter, () => !message.Long);

				await _totalRewardService.CheckTotal(userId, statuses, achievements, statusesChanged, achievementsChanged);
			}
		}
	}
}