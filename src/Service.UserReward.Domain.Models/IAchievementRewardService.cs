using System.Collections.Generic;
using Service.Core.Domain.Models.Constants;
using Service.EducationProgress.Domain.Models;
using Service.EducationProgress.Grpc.ServiceBusModels;

namespace Service.UserReward.Domain.Models
{
	public interface IAchievementRewardService
	{
		bool CheckByProgress(SetProgressInfoServiceBusModel model, EducationProgressDto[] educationProgress, List<UserAchievement> achievements);

		bool CheckTotal(IEnumerable<StatusDto> statuses, List<UserAchievement> achievements);

		bool CheckAllStatusesAchievement(IEnumerable<StatusDto> statuses, List<UserAchievement> achievements);
	}
}