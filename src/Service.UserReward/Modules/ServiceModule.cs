﻿using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.Core.Client.Services;
using Service.EducationProgress.Client;
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
			builder.RegisterServerKeyValueClient(Program.Settings.ServerKeyValueServiceUrl, Program.LogFactory.CreateLogger(typeof(ServerKeyValueClientFactory)));
			builder.RegisterEducationProgressClient(Program.Settings.EducationProgressServiceUrl);

			builder.RegisterType<StatusRewardService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<AchievementRewardService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<DtoRepository>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<TotalRewardService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<SystemClock>().AsImplementedInterfaces().SingleInstance();

			MyServiceBusTcpClient serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.ServiceBusReader), Program.LogFactory);
			builder.RegisterMyServiceBusSubscriberBatch<ProfilingFinishedServiceBusModel>(serviceBusClient, ProfilingFinishedServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);
			builder.RegisterMyServiceBusSubscriberBatch<SetProgressInfoServiceBusModel>(serviceBusClient, SetProgressInfoServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);
			builder.RegisterMyServiceBusSubscriberBatch<UserAccountFilledServiceBusModel>(serviceBusClient, UserAccountFilledServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);
			builder.RegisterMyServiceBusSubscriberBatch<RetryUsedServiceBusModel>(serviceBusClient, RetryUsedServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);
			builder.RegisterMyServiceBusSubscriberBatch<UserProgressUpdatedServiceBusModel>(serviceBusClient, UserProgressUpdatedServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);
			builder.RegisterMyServiceBusSubscriberBatch<UserTimeChangedServiceBusModel>(serviceBusClient, UserTimeChangedServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);
			builder.RegisterMyServiceBusSubscriberBatch<MarketProductPurchasedServiceBusModel>(serviceBusClient, MarketProductPurchasedServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);

			builder.RegisterType<ProfilingFinishedNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<SetProgressInfoNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<UserAccountFilledNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<RetryUsedNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<UserProgressUpdatedNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<UserTimeChangedNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<MarketProductPurchasedNotificator>().AutoActivate().SingleInstance();

			var tcpServiceBus = new MyServiceBusTcpClient(() => Program.Settings.ServiceBusWriter, "MyJetEducation Service.UserReward");

			builder
				.Register(context => new MyServiceBusPublisher<UserRewardedServiceBusModel>(tcpServiceBus, UserRewardedServiceBusModel.TopicName, false))
				.As<IServiceBusPublisher<UserRewardedServiceBusModel>>()
				.SingleInstance();

			tcpServiceBus.Start();
		}
	}
}