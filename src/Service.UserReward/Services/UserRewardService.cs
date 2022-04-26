using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyJetWallet.Sdk.ServiceBus;
using Service.Core.Client.Constants;
using Service.Core.Client.Models;
using Service.Education.Structure;
using Service.ServiceBus.Models;
using Service.UserReward.Grpc;
using Service.UserReward.Grpc.Models;
using Service.UserReward.Helpers;
using Service.UserReward.Mappers;
using Service.UserReward.Models;

namespace Service.UserReward.Services
{
	public class UserRewardService : IUserRewardService
	{
		private readonly IDtoRepository _dtoRepository;
		private readonly ITotalRewardService _totalRewardService;
		private readonly IServiceBusPublisher<UserRewardedServiceBusModel> _publisher;

		public UserRewardService(IDtoRepository dtoRepository, ITotalRewardService totalRewardService, IServiceBusPublisher<UserRewardedServiceBusModel> publisher)
		{
			_dtoRepository = dtoRepository;
			_totalRewardService = totalRewardService;
			_publisher = publisher;
		}

		public async ValueTask<UserStatusesGrpcResponse> GetUserStatusesAsync(GetUserStatusesGrpcRequest request) =>
			(await GetStatusList(request.UserId)).ToGrpcModel();

		public async ValueTask<UserLastStatusGrpcResponse> GetUserLastStatusAsync(GetUserLastStatusGrpcRequest request) => new UserLastStatusGrpcResponse
		{
			Status = (await GetStatusList(request.UserId)).LastOrDefault().ToGrpcModel()
		};

		private async ValueTask<List<StatusDto>> GetStatusList(string userId) =>
			await _dtoRepository.GetStatuses(userId);

		public async ValueTask<UserAchievementsGrpcResponse> GetUserAchievementsAsync(GetUserAchievementsGrpcRequest request) =>
			(await _dtoRepository.GetAchievements(request.UserId)).ToGrpcModel();

		public async ValueTask<UserAchievementsGrpcResponse> GetUserNewAchievementsAsync(GetUserAchievementsGrpcRequest request)
		{
			INewAchievementsDto achievements = request.Unit == null
				? (INewAchievementsDto) await _dtoRepository.GetNewAchievementsTutorial(request.UserId)
				: await _dtoRepository.GetNewAchievementsUnit(request.UserId);

			return new UserAchievementsGrpcResponse
			{
				Items = achievements?.Achievements.ToArray()
			};
		}

		/// <summary>
		///     впервые зашел в marketplace (вне онбординга)
		/// </summary>
		public async ValueTask<CommonGrpcResponse> VisitMarketplace(VisitMarketplaceGrpcRequest request) =>
			await Process(request.UserId, (statuses, achievements) => achievements.SetAchievement(UserAchievement.Eyescatter));

		public async ValueTask<CommonGrpcResponse> LearningStartedAsync(LearningStartedGrpcRequest request) => await Process(request.UserId, (statuses, achievements) =>
		{
			//начал первый урок
			achievements.SetAchievement(UserAchievement.Ignition, () => request.Tutorial == EducationTutorial.PersonalFinance && request.Unit == 1 && request.Task == 1);

			//начато изучение 2-й дисциплины
			statuses.SetStatus(UserStatus.SecondYearStudent, () => request.Tutorial == EducationTutorial.BehavioralFinance && request.Unit == null && request.Task == null);
		});

		private async ValueTask<CommonGrpcResponse> Process(string userId, Action<StatusInfo, AchievementInfo> action)
		{
			(StatusInfo statuses, AchievementInfo achievements) = await _dtoRepository.GetAll(userId);

			action.Invoke(statuses, achievements);

			CommonGrpcResponse commonGrpcResponse = await _totalRewardService.CheckTotal(userId, statuses, achievements);

			await PublishHelper.TryPublishUserRewarded(_publisher, userId, statuses, achievements);

			return commonGrpcResponse;
		}
	}
}