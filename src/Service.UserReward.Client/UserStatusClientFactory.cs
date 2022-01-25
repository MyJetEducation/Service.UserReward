using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.UserReward.Grpc;

namespace Service.UserReward.Client
{
    [UsedImplicitly]
    public class UserRewardClientFactory : MyGrpcClientFactory
    {
        public UserRewardClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IUserRewardService GetUserRewardService() => CreateGrpcService<IUserRewardService>();
    }
}
