using System.Threading.Tasks;
using Service.EducationProgress.Domain.Models;
using Service.EducationProgress.Grpc.ServiceBusModels;
using Service.UserReward.Models;

namespace Service.UserReward.Services
{
	public interface IStatusRewardService
	{
		ValueTask CheckByProgress(SetProgressInfoServiceBusModel model, EducationProgressDto[] educationProgress, StatusInfo statusInfo);

		void CheckTotal(StatusInfo statusInfo, AchievementInfo achievementsInfo);
	}
}