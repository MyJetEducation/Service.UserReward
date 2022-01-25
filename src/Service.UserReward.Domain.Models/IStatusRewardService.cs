using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Core.Domain.Models.Constants;
using Service.EducationProgress.Domain.Models;
using Service.EducationProgress.Grpc.ServiceBusModels;

namespace Service.UserReward.Domain.Models
{
	public interface IStatusRewardService
	{
		Task<bool> CheckByProgress(SetProgressInfoServiceBusModel model, EducationProgressDto[] educationProgress, List<StatusDto> statuses);

		bool CheckTotal(List<StatusDto> statuses, IEnumerable<UserAchievement> achievements);
	}
}