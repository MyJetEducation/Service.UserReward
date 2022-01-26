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

		public async ValueTask<UserStatusesGrpcResponse> GetUserStatusesAsync(GetUserStatusesGrpcRequset request) =>
			(await _dtoRepository.GetStatuses(request.UserId)).ToGrpcModel();

		public async ValueTask<UserAchievementsGrpcResponse> GetUserAchievementsAsync(GetUserAchievementsGrpcRequset request) =>
			(await _dtoRepository.GetAchievements(request.UserId)).ToGrpcModel();

		public async ValueTask<UserAchievementsGrpcResponse> GetUserNewUnitAchievementsAsync(GetUserAchievementsGrpcRequset request) =>
			(await _dtoRepository.GetNewAchievements(request.UserId)).ToGrpcModel();

		/// <summary>
		///     повзаимодействовал с персонажем
		/// </summary>
		public async ValueTask<CommonGrpcResponse> MascotInteractionAsync(MascotInteractionGrpcRequset requset) =>
			await Process(requset.UserId, (statuses, achievements) => achievements.SetAchievement(UserAchievement.FirstTouch));

		/// <summary>
		///     впервые зашел в marketplace (вне онбординга)
		/// </summary>
		public async ValueTask<CommonGrpcResponse> VisitMarketplace(VisitMarketplaceGrpcRequset requset) =>
			await Process(requset.UserId, (statuses, achievements) => achievements.SetAchievement(UserAchievement.Eyescatter));

		public async ValueTask<CommonGrpcResponse> LearningStartedAsync(LearningStartedGrpcRequset requset) =>
			await Process(requset.UserId, (statuses, achievements) =>
			{
				//начал первый урок
				achievements.SetAchievement(UserAchievement.Ignition, () => requset.Tutorial == EducationTutorial.PersonalFinance && requset.Unit == 1 && requset.Task == 1);

				//начато изучение 2-й дисциплины
				statuses.SetStatus(UserStatus.SecondYearStudent, () => requset.Tutorial == EducationTutorial.BehavioralFinance && requset.Unit == null && requset.Task == null);
			});

		private async ValueTask<CommonGrpcResponse> Process(Guid? userId, Action<StatusInfo, AchievementInfo> action)
		{
			(StatusInfo statuses, AchievementInfo achievements) = await _dtoRepository.GetAll(userId);

			action.Invoke(statuses, achievements);

			return await _totalRewardService.CheckTotal(userId, statuses, achievements);
		}
	}
}