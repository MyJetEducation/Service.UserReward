using Autofac;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.EducationProgress.Grpc.ServiceBusModels;
using Service.ServerKeyValue.Client;
using Service.UserProgress.Domain.Models;
using Service.UserReward.Grpc.ServiceBusModels;
using Service.UserReward.Jobs;
using Service.UserReward.Services;

namespace Service.UserReward.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterKeyValueClient(Program.Settings.ServerKeyValueServiceUrl);
			builder.RegisterType<StatusRewardService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<AchievementRewardService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<DtoRepository>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<TotalRewardService>().AsImplementedInterfaces().SingleInstance();

			MyServiceBusTcpClient serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.ServiceBusReader), Program.LogFactory);
			const string queueName = "MyJetEducation-UserReward";
			const TopicQueueType queryType = TopicQueueType.Permanent;

			builder.RegisterMyServiceBusSubscriberBatch<ProfilingFinishedServiceBusModel>(serviceBusClient, ProfilingFinishedServiceBusModel.TopicName, queueName, queryType);
			builder.RegisterMyServiceBusSubscriberBatch<SetProgressInfoServiceBusModel>(serviceBusClient, SetProgressInfoServiceBusModel.TopicName, queueName, queryType);
			builder.RegisterMyServiceBusSubscriberBatch<UserAccountFilledServiceBusModel>(serviceBusClient, UserAccountFilledServiceBusModel.TopicName, queueName, queryType);
			builder.RegisterMyServiceBusSubscriberBatch<RetryUsedServiceBusModel>(serviceBusClient, RetryUsedServiceBusModel.TopicName, queueName, queryType);
			builder.RegisterMyServiceBusSubscriberBatch<UserProgressUpdatedServiceBusModel>(serviceBusClient, UserProgressUpdatedServiceBusModel.TopicName, queueName, queryType);

			builder.RegisterType<ProfilingFinishedNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<SetProgressInfoNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<UserAccountFilledNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<RetryUsedNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<UserProgressUpdatedNotificator>().AutoActivate().SingleInstance();
		}
	}
}