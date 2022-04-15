using Service.EducationProgress.Grpc.Models;
using Service.ServiceBus.Models;
using Service.UserReward.Models;

namespace Service.UserReward.Services
{
	public interface IAchievementRewardService
	{
		void CheckByProgress(SetProgressInfoServiceBusModel model, EducationProgressTaskDataGrpcModel[] educationProgress, AchievementInfo achievementsInfo);

		void CheckTotal(StatusInfo statusInfo, AchievementInfo achievementsInfo);

		void CheckAllStatusesAchievement(StatusInfo statusInfo, AchievementInfo achievementsInfo);
	}
}