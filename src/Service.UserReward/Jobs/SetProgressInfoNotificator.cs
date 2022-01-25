using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.Core.Domain.Models.Constants;
using Service.EducationProgress.Domain.Models;
using Service.EducationProgress.Grpc.ServiceBusModels;
using Service.UserReward.Domain.Models;
using Service.UserReward.Services;

namespace Service.UserReward.Jobs
{
	public class SetProgressInfoNotificator
	{
		private readonly ILogger<SetProgressInfoNotificator> _logger;
		private readonly IStatusRewardService _statusRewardService;
		private readonly IAchievementRewardService _achievementRewardService;
		private readonly IDtoRepository _dtoRepository;
		private readonly ITotalRewardService _totalRewardService;

		public SetProgressInfoNotificator(ILogger<SetProgressInfoNotificator> logger,
			ISubscriber<IReadOnlyList<SetProgressInfoServiceBusModel>> subscriber,
			IStatusRewardService statusRewardService,
			IAchievementRewardService achievementRewardService,
			IDtoRepository dtoRepository)
		{
			_logger = logger;
			_statusRewardService = statusRewardService;
			_achievementRewardService = achievementRewardService;
			_dtoRepository = dtoRepository;
			_totalRewardService = new TotalRewardService(_statusRewardService, _achievementRewardService, _dtoRepository);
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<SetProgressInfoServiceBusModel> events)
		{
			foreach (SetProgressInfoServiceBusModel message in events)
			{
				Guid? userId = message.UserId;
				(List<StatusDto> statuses, List<UserAchievement> achievements) = await _dtoRepository.GetAll(userId);
				EducationProgressDto[] educationProgress = await _dtoRepository.GetEducationProgress(userId);

				UserAchievement[] wasAchievements = achievements
					.ToArray()
					.Clone() as UserAchievement[]
					?? Array.Empty<UserAchievement>();

				_logger.LogDebug("Handled {model} for {user}", nameof(SetProgressInfoServiceBusModel), userId);

				bool statusesChanged = await _statusRewardService.CheckByProgress(message, educationProgress, statuses);
				bool achievementsChanged = _achievementRewardService.CheckByProgress(message, educationProgress, achievements);

				await _totalRewardService.CheckTotal(userId, statuses, achievements, statusesChanged, achievementsChanged);

				await ProcessNewAchievements(userId, message, achievements, wasAchievements);
			}
		}

		private async Task ProcessNewAchievements(Guid? userId, SetProgressInfoServiceBusModel message, IEnumerable<UserAchievement> achievements, UserAchievement[] wasAchievements)
		{
			if (message.IsRetry)
				return;

			NewAchievementsDto newAchievementsDto = await _dtoRepository.GetNewAchievements(userId);

			if (message.Tutorial != newAchievementsDto.Tutorial || message.Unit != newAchievementsDto.Unit)
				newAchievementsDto = new NewAchievementsDto(message.Tutorial, message.Unit);

			UserAchievement[] newAchievements = newAchievementsDto.Achievements;
			int wasLength = newAchievements.Length;

			newAchievementsDto.Achievements = achievements.Except(wasAchievements).ToArray();

			if (wasLength != newAchievements.Length)
				await _dtoRepository.SetNewAchievements(userId, newAchievementsDto);
		}
	}
}