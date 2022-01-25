using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.Core.Domain.Models.Constants;
using Service.UserProgress.Domain.Models;
using Service.UserReward.Domain.Models;
using Service.UserReward.Helpers;
using IDtoRepository = Service.UserReward.Domain.Models.IDtoRepository;

namespace Service.UserReward.Jobs
{
	public class UserProgressUpdatedNotificator
	{
		private readonly ILogger<UserProgressUpdatedNotificator> _logger;
		private readonly IDtoRepository _dtoRepository;
		private readonly ITotalRewardService _totalRewardService;

		public UserProgressUpdatedNotificator(ILogger<UserProgressUpdatedNotificator> logger,
			ISubscriber<IReadOnlyList<UserProgressUpdatedServiceBusModel>> subscriber,
			IDtoRepository dtoRepository,
			ITotalRewardService totalRewardService)
		{
			_logger = logger;
			_dtoRepository = dtoRepository;
			_totalRewardService = totalRewardService;
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<UserProgressUpdatedServiceBusModel> events)
		{
			foreach (UserProgressUpdatedServiceBusModel message in events)
			{
				Guid? userId = message.UserId;
				int habitCount = message.HabitCount;
				(List<StatusDto> statuses, List<UserAchievement> achievements) = await _dtoRepository.GetAll(userId);
				var statusesChanged = false;
				var achievementsChanged = false;

				_logger.LogDebug("Handled {model} for {user}", nameof(UserProgressUpdatedServiceBusModel), userId);

				//за пройденные задачи определенного типа (шаг изменения = 2 за сформировавшиеся привычки)
				void AddStrategistStatus(int level) => statusesChanged = statuses.SetStatus(UserStatus.Strategist, level);

				switch (habitCount)
				{
					case 1:
						//освоил одну привычку
						achievementsChanged = achievements.SetAchievement(UserAchievement.Habitant);
						break;
					case 2:
						AddStrategistStatus(1);
						break;
					case 4:
						AddStrategistStatus(2);
						break;
					case 6:
						AddStrategistStatus(3);
						break;
					case 8:
						AddStrategistStatus(4);
						break;
					case 9:
						//освоил все привычки
						achievementsChanged = achievements.SetAchievement(UserAchievement.TheHabitMaster);
						AddStrategistStatus(5);
						break;
				}

				await _totalRewardService.CheckTotal(userId, statuses, achievements, statusesChanged, achievementsChanged);
			}
		}
	}
}