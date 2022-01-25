using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.Core.Domain.Models.Constants;
using Service.UserProfile.Grpc.ServiceBusModel;
using Service.UserReward.Domain.Models;
using Service.UserReward.Helpers;

namespace Service.UserReward.Jobs
{
	public class UserAccountFilledNotificator
	{
		private readonly ILogger<UserAccountFilledNotificator> _logger;
		private readonly IDtoRepository _dtoRepository;
		private readonly ITotalRewardService _totalRewardService;

		public UserAccountFilledNotificator(ILogger<UserAccountFilledNotificator> logger,
			ISubscriber<IReadOnlyList<UserAccountFilledServiceBusModel>> subscriber,
			IDtoRepository dtoRepository,
			ITotalRewardService totalRewardService)
		{
			_logger = logger;
			_dtoRepository = dtoRepository;
			_totalRewardService = totalRewardService;
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<UserAccountFilledServiceBusModel> events)
		{
			foreach (UserAccountFilledServiceBusModel message in events)
			{
				Guid? userId = message.UserId;
				(List<StatusDto> statuses, List<UserAchievement> achievements) = await _dtoRepository.GetAll(userId);

				_logger.LogDebug("Handled {model} for {user}", nameof(UserAccountFilledServiceBusModel), userId);

				//внес все персональные данные в профиле
				bool achievementsChanged = achievements.SetAchievement(UserAchievement.Complaisance);

				await _totalRewardService.CheckTotal(userId, statuses, achievements, false, achievementsChanged);
			}
		}
	}
}