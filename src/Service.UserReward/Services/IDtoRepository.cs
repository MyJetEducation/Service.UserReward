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
		ValueTask<EducationProgressDto[]> GetEducationProgress(Guid? userId);

		ValueTask<(StatusInfo, AchievementInfo)> GetAll(Guid? userId);

		ValueTask<List<StatusDto>> GetStatuses(Guid? userId);
		ValueTask<bool> SetStatuses(Guid? userId, StatusInfo statuses);

		ValueTask<List<UserAchievement>> GetAchievements(Guid? userId);
		ValueTask<bool> SetAchievements(Guid? userId, AchievementInfo achievements);

		ValueTask<NewAchievementsTutorialDto> GetNewAchievementsTutorial(Guid? userId);
		ValueTask<NewAchievementsUnitDto> GetNewAchievementsUnit(Guid? userId);
		ValueTask<CommonGrpcResponse> SetNewAchievements(Guid? userId, NewAchievementsTutorialDto tutorialDto, NewAchievementsUnitDto unitDto);

		ValueTask<TestTasks100PrcDto> GetTestTasks100Prc(Guid? userId);
		ValueTask<CommonGrpcResponse> ClearTestTasks100Prc(Guid? userId);
	}
}