using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Core.Client.Constants;
using Service.Core.Client.Models;
using Service.EducationProgress.Domain.Models;
using Service.UserReward.Models;

namespace Service.UserReward.Services
{
	public interface IDtoRepository
	{
		ValueTask<(StatusInfo, AchievementInfo)> GetAll(string userId);

		ValueTask<List<StatusDto>> GetStatuses(string userId);
		ValueTask<bool> SetStatuses(string userId, StatusInfo statuses);

		ValueTask<List<UserAchievement>> GetAchievements(string userId);
		ValueTask<bool> SetAchievements(string userId, AchievementInfo achievements);

		ValueTask<NewAchievementsTutorialDto> GetNewAchievementsTutorial(string userId);
		ValueTask<NewAchievementsUnitDto> GetNewAchievementsUnit(string userId);
		ValueTask<CommonGrpcResponse> SetNewAchievements(string userId, NewAchievementsTutorialDto tutorialDto, NewAchievementsUnitDto unitDto);

		ValueTask<TestTasks100PrcDto> GetTestTasks100Prc(string userId);
		ValueTask<CommonGrpcResponse> ClearTestTasks100Prc(string userId);
	}
}