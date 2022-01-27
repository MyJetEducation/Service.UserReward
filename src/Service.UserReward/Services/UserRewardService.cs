using System;
using System.Threading.Tasks;
using Service.Core.Client.Constants;
using Service.Core.Client.Education;
using Service.Core.Client.Models;
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

		public UserRewardService(IDtoRepository dtoRepository, ITotalRewardService totalRewardService)
		{
			_dtoRepository = dtoRepository;
			_totalRewardService = totalRewardService;
		}

		public async ValueTask<UserStatusesGrpcResponse> GetUserStatusesAsync(GetUserStatusesGrpcRequest request) =>
			(await _dtoRepository.GetStatuses(request.UserId)).ToGrpcModel();

		public async ValueTask<UserAchievementsGrpcResponse> GetUserAchievementsAsync(GetUserAchievementsGrpcRequest request) =>
			(await _dtoRepository.GetAchievements(request.UserId)).ToGrpcModel();

		public async ValueTask<UserAchievementsGrpcResponse> GetUserNewUnitAchievementsAsync(GetUserAchievementsGrpcRequest request) =>
			(await _dtoRepository.GetNewAchievements(request.UserId)).ToGrpcModel();

		/// <summary>
		///     повзаимодействовал с персонажем
		/// </summary>
		public async ValueTask<CommonGrpcResponse> MascotInteractionAsync(MascotInteractionGrpcRequest request) =>
			await Process(request.UserId, (statuses, achievements) => achievements.SetAchievement(UserAchievement.FirstTouch));

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

		private async ValueTask<CommonGrpcResponse> Process(Guid? userId, Action<StatusInfo, AchievementInfo> action)
		{
			(StatusInfo statuses, AchievementInfo achievements) = await _dtoRepository.GetAll(userId);

			action.Invoke(statuses, achievements);

			return await _totalRewardService.CheckTotal(userId, statuses, achievements);
		}
	}
}