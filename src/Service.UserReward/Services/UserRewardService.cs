using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Core.Domain.Models.Constants;
using Service.Core.Domain.Models.Education;
using Service.Core.Grpc.Models;
using Service.UserReward.Domain.Models;
using Service.UserReward.Grpc;
using Service.UserReward.Grpc.Models;
using Service.UserReward.Helpers;
using Service.UserReward.Mappers;

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

		public async ValueTask<CommonGrpcResponse> MascotInteractionAsync(MascotInteractionGrpcRequset requset)
		{
			Guid? userId = requset.UserId;
			(List<StatusDto> statuses, List<UserAchievement> achievements) = await _dtoRepository.GetAll(userId);

			bool achievementsChanged = achievements.SetAchievement(UserAchievement.FirstTouch);

			return await _totalRewardService.CheckTotal(userId, statuses, achievements, false, achievementsChanged);
		}

		public async ValueTask<CommonGrpcResponse> VisitMarketplace(VisitMarketplaceGrpcRequset requset)
		{
			Guid? userId = requset.UserId;
			(List<StatusDto> statuses, List<UserAchievement> achievements) = await _dtoRepository.GetAll(userId);

			bool achievementsChanged = achievements.SetAchievement(UserAchievement.Eyescatter);

			return await _totalRewardService.CheckTotal(userId, statuses, achievements, false, achievementsChanged);
		}

		public async ValueTask<CommonGrpcResponse> LearningStartedAsync(LearningStartedGrpcRequset requset)
		{
			Guid? userId = requset.UserId;
			(List<StatusDto> statuses, List<UserAchievement> achievements) = await _dtoRepository.GetAll(userId);
			var statusesChanged = false;
			var achievementsChanged = false;

			switch (requset.Tutorial)
			{
				case EducationTutorial.PersonalFinance when requset.Unit == 1 && requset.Task == 1:
					achievementsChanged = achievements.SetAchievement(UserAchievement.Ignition);
					break;
				case EducationTutorial.BehavioralFinance when requset.Unit == null && requset.Task == null:
					statusesChanged = statuses.SetStatus(UserStatus.SecondYearStudent);
					break;
				default:
					return CommonGrpcResponse.Fail;
			}

			return await _totalRewardService.CheckTotal(userId, statuses, achievements, statusesChanged, achievementsChanged);
		}
	}
}