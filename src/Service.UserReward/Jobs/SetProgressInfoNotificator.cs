using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.EducationProgress.Domain.Models;
using Service.EducationProgress.Grpc.ServiceBusModels;
using Service.UserReward.Models;
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
				(StatusInfo statuses, AchievementInfo achievements) = await _dtoRepository.GetAll(userId);
				EducationProgressDto[] educationProgress = await _dtoRepository.GetEducationProgress(userId);

				_logger.LogDebug("ServiceBus Notificator {notificator} handled message for {user}", GetType().Name, userId);

				await _statusRewardService.CheckByProgress(message, educationProgress, statuses);
				_achievementRewardService.CheckByProgress(message, educationProgress, achievements);
				await _totalRewardService.CheckTotal(userId, statuses, achievements);
				await ProcessNewAchievements(userId, message, achievements);
			}
		}

		private async Task ProcessNewAchievements(Guid? userId, SetProgressInfoServiceBusModel message, AchievementInfo achievements)
		{
			if (message.IsRetry)
				return;

			NewAchievementsDto newAchievementsDto = await _dtoRepository.GetNewAchievements(userId);

			if (newAchievementsDto == null || message.Tutorial != newAchievementsDto.Tutorial || message.Unit != newAchievementsDto.Unit)
				newAchievementsDto = new NewAchievementsDto(message.Tutorial, message.Unit);

			newAchievementsDto.Achievements.AddRange(achievements.NewItems);

			await _dtoRepository.SetNewAchievements(userId, newAchievementsDto);
		}
	}
}