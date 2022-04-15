using System.Threading.Tasks;
using Service.EducationProgress.Grpc.Models;
using Service.ServiceBus.Models;
using Service.UserReward.Models;

namespace Service.UserReward.Services
{
	public interface IStatusRewardService
	{
		ValueTask CheckByProgress(SetProgressInfoServiceBusModel model, EducationProgressTaskDataGrpcModel[] educationProgress, StatusInfo statusInfo);

		void CheckTotal(StatusInfo statusInfo, AchievementInfo achievementsInfo);
	}
}