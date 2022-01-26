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

		ValueTask<NewAchievementsDto> GetNewAchievements(Guid? userId);
		ValueTask<CommonGrpcResponse> SetNewAchievements(Guid? userId, NewAchievementsDto dto);

		ValueTask<TestTasks100PrcDto> GetTestTasks100Prc(Guid? userId);
		ValueTask<CommonGrpcResponse> ClearTestTasks100Prc(Guid? userId);
	}
}