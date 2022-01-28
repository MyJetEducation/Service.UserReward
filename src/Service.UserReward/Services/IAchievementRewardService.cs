using Service.EducationProgress.Domain.Models;
using Service.ServiceBus.Models;
using Service.UserReward.Models;

namespace Service.UserReward.Services
{
	public interface IAchievementRewardService
	{
		void CheckByProgress(SetProgressInfoServiceBusModel model, EducationProgressDto[] educationProgress, AchievementInfo achievementsInfo);

		void CheckTotal(StatusInfo statusInfo, AchievementInfo achievementsInfo);

		void CheckAllStatusesAchievement(StatusInfo statusInfo, AchievementInfo achievementsInfo);
	}
}