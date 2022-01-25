using Autofac;
using Service.UserReward.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.UserReward.Client
{
    public static class AutofacHelper
    {
        public static void RegisterUserRewardClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new UserRewardClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetUserRewardService()).As<IUserRewardService>().SingleInstance();
        }
    }
}
