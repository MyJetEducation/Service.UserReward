using System.ServiceModel;
using System.Threading.Tasks;
using Service.Core.Client.Models;
using Service.UserReward.Grpc.Models;

namespace Service.UserReward.Grpc
{
	[ServiceContract]
	public interface IUserRewardService
	{
		[OperationContract]
		ValueTask<UserStatusesGrpcResponse> GetUserStatusesAsync(GetUserStatusesGrpcRequset request);

		[OperationContract]
		ValueTask<UserAchievementsGrpcResponse> GetUserAchievementsAsync(GetUserAchievementsGrpcRequset request);

		[OperationContract]
		ValueTask<UserAchievementsGrpcResponse> GetUserNewUnitAchievementsAsync(GetUserAchievementsGrpcRequset request);

		[OperationContract]
		ValueTask<CommonGrpcResponse> MascotInteractionAsync(MascotInteractionGrpcRequset requset);

		[OperationContract]
		ValueTask<CommonGrpcResponse> LearningStartedAsync(LearningStartedGrpcRequset requset);
		
		[OperationContract]
		ValueTask<CommonGrpcResponse> VisitMarketplace(VisitMarketplaceGrpcRequset requset);
	}
}