using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.Core.Client.Constants;
using Service.Core.Client.Extensions;
using Service.EducationProgress.Grpc;
using Service.EducationProgress.Grpc.Models;
using Service.ServiceBus.Models;
using Service.UserReward.Helpers;
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
		private readonly IEducationProgressService _educationProgressService;
		private readonly IServiceBusPublisher<UserRewardedServiceBusModel> _publisher;

		public SetProgressInfoNotificator(ILogger<SetProgressInfoNotificator> logger,
			ISubscriber<IReadOnlyList<SetProgressInfoServiceBusModel>> subscriber,
			IStatusRewardService statusRewardService,
			IAchievementRewardService achievementRewardService,
			IDtoRepository dtoRepository,
			IEducationProgressService educationProgressService,
			IServiceBusPublisher<UserRewardedServiceBusModel> publisher)
		{
			_logger = logger;
			_statusRewardService = statusRewardService;
			_achievementRewardService = achievementRewardService;
			_dtoRepository = dtoRepository;
			_educationProgressService = educationProgressService;
			_publisher = publisher;
			_totalRewardService = new TotalRewardService(_statusRewardService, _achievementRewardService, _dtoRepository);
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<SetProgressInfoServiceBusModel> events)
		{
			foreach (SetProgressInfoServiceBusModel message in events)
			{
				string userId = message.UserId;

				_logger.LogDebug("ServiceBus Notificator {notificator} handled message for {user}", GetType().Name, userId);

				(StatusInfo statuses, AchievementInfo achievements) = await _dtoRepository.GetAll(userId);

				EducationProgressTaskDataGrpcModel[] educationProgress = (await _educationProgressService.GetProgressData(new GetProgressDataGrpcRequest {UserId = userId}))?.Items;
				if (educationProgress.IsNullOrEmpty())
				{
					_logger.LogError("No progress recieved for user: {userId}, exiting...", userId);
					return;
				}

				await _statusRewardService.CheckByProgress(message, educationProgress, statuses);
				_achievementRewardService.CheckByProgress(message, educationProgress, achievements);
				await _totalRewardService.CheckTotal(userId, statuses, achievements);
				await ProcessNewAchievements(userId, message, achievements);
				await PublishHelper.TryPublishUserRewarded(_publisher, userId, statuses, achievements);
			}
		}

		private async ValueTask ProcessNewAchievements(string userId, SetProgressInfoServiceBusModel message, AchievementInfo achievements)
		{
			if (message.IsRetry)
				return;

			List<UserAchievement> newItems = achievements.NewItems;

			NewAchievementsUnitDto forUnitDto = await _dtoRepository.GetNewAchievementsUnit(userId);
			if (forUnitDto == null || message.Tutorial != forUnitDto.Tutorial || message.Unit != forUnitDto.Unit)
				forUnitDto = new NewAchievementsUnitDto(message.Tutorial, message.Unit);
			forUnitDto.Achievements.AddRange(newItems);

			NewAchievementsTutorialDto forTutorialDto = await _dtoRepository.GetNewAchievementsTutorial(userId);
			if (forTutorialDto == null || message.Tutorial != forTutorialDto.Tutorial)
				forTutorialDto = new NewAchievementsTutorialDto(message.Tutorial);
			forTutorialDto.Achievements.AddRange(newItems);

			await _dtoRepository.SetNewAchievements(userId, forTutorialDto, forUnitDto);
		}
	}
}