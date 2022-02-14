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
		ValueTask<UserStatusesGrpcResponse> GetUserStatusesAsync(GetUserStatusesGrpcRequest request);

		[OperationContract]
		ValueTask<UserAchievementsGrpcResponse> GetUserAchievementsAsync(GetUserAchievementsGrpcRequest request);

		[OperationContract]
		ValueTask<UserAchievementsGrpcResponse> GetUserNewAchievementsAsync(GetUserAchievementsGrpcRequest request);

		[OperationContract]
		ValueTask<CommonGrpcResponse> MascotInteractionAsync(MascotInteractionGrpcRequest request);

		[OperationContract]
		ValueTask<CommonGrpcResponse> LearningStartedAsync(LearningStartedGrpcRequest request);
		
		[OperationContract]
		ValueTask<CommonGrpcResponse> VisitMarketplace(VisitMarketplaceGrpcRequest request);
	}
}