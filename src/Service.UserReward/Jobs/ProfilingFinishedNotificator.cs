using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.Core.Client.Constants;
using Service.ServiceBus.Models;
using Service.UserReward.Helpers;
using Service.UserReward.Services;

namespace Service.UserReward.Jobs
{
	public class ProfilingFinishedNotificator : NotificatorBase
	{
		public ProfilingFinishedNotificator(ILogger<ProfilingFinishedNotificator> logger,
			ISubscriber<IReadOnlyList<ProfilingFinishedServiceBusModel>> subscriber,
			IDtoRepository dtoRepository,
			ITotalRewardService totalRewardService, IServiceBusPublisher<UserRewardedServiceBusModel> publisher) 
			: base(dtoRepository, totalRewardService, logger, publisher) =>
				subscriber.Subscribe(HandleEvent);

		private async ValueTask HandleEvent(IReadOnlyList<ProfilingFinishedServiceBusModel> events)
		{
			foreach (ProfilingFinishedServiceBusModel message in events)
			{
				await Process(message.UserId, (statuses, achievements) =>
				{
					//пройдено длинное профилирование
					statuses.SetStatus(UserStatus.MasterOfOpenness, () => message.Long);
				});
			}
		}
	}
}