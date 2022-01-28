using Autofac;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.ServerKeyValue.Client;
using Service.ServiceBus.Models;
using Service.UserReward.Jobs;
using Service.UserReward.Services;

namespace Service.UserReward.Modules
{
	public class ServiceModule : Module
	{
		private const string QueueName = "MyJetEducation-UserReward";

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterKeyValueClient(Program.Settings.ServerKeyValueServiceUrl);
			builder.RegisterType<StatusRewardService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<AchievementRewardService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<DtoRepository>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<TotalRewardService>().AsImplementedInterfaces().SingleInstance();

			MyServiceBusTcpClient serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.ServiceBusReader), Program.LogFactory);
			builder.RegisterMyServiceBusSubscriberBatch<ProfilingFinishedServiceBusModel>(serviceBusClient, ProfilingFinishedServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);
			builder.RegisterMyServiceBusSubscriberBatch<SetProgressInfoServiceBusModel>(serviceBusClient, SetProgressInfoServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);
			builder.RegisterMyServiceBusSubscriberBatch<UserAccountFilledServiceBusModel>(serviceBusClient, UserAccountFilledServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);
			builder.RegisterMyServiceBusSubscriberBatch<RetryUsedServiceBusModel>(serviceBusClient, RetryUsedServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);
			builder.RegisterMyServiceBusSubscriberBatch<UserProgressUpdatedServiceBusModel>(serviceBusClient, UserProgressUpdatedServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);

			builder.RegisterType<ProfilingFinishedNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<SetProgressInfoNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<UserAccountFilledNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<RetryUsedNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<UserProgressUpdatedNotificator>().AutoActivate().SingleInstance();
		}
	}
}