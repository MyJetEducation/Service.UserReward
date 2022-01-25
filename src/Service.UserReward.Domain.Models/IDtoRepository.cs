using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Core.Domain.Models.Constants;
using Service.Core.Grpc.Models;
using Service.EducationProgress.Domain.Models;

namespace Service.UserReward.Domain.Models
{
	public interface IDtoRepository
	{
		ValueTask<EducationProgressDto[]> GetEducationProgress(Guid? userId);

		ValueTask<(List<StatusDto>, List<UserAchievement>)> GetAll(Guid? userId);

		ValueTask<List<StatusDto>> GetStatuses(Guid? userId);
		ValueTask<bool> SetStatuses(Guid? userId, IEnumerable<StatusDto> dtos);

		ValueTask<List<UserAchievement>> GetAchievements(Guid? userId);
		ValueTask<bool> SetAchievements(Guid? userId, IEnumerable<UserAchievement> achievements);

		ValueTask<NewAchievementsDto> GetNewAchievements(Guid? userId);
		ValueTask<CommonGrpcResponse> SetNewAchievements(Guid? userId, NewAchievementsDto dto);

		ValueTask<TestTasks100PrcDto> GetTestTasks100Prc(Guid? userId);
		ValueTask<CommonGrpcResponse> ClearTestTasks100Prc(Guid? userId);
	}
}