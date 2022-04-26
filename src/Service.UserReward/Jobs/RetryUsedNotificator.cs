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
	public class RetryUsedNotificator : NotificatorBase
	{
		public RetryUsedNotificator(ILogger<RetryUsedNotificator> logger,
			ISubscriber<IReadOnlyList<RetryUsedServiceBusModel>> subscriber,
			IDtoRepository dtoRepository,
			ITotalRewardService totalRewardService, IServiceBusPublisher<UserRewardedServiceBusModel> publisher) 
			: base(dtoRepository, totalRewardService, logger, publisher) =>
				subscriber.Subscribe(HandleEvent);

		private async ValueTask HandleEvent(IReadOnlyList<RetryUsedServiceBusModel> events)
		{
			foreach (RetryUsedServiceBusModel message in events)
			{
				await Process(message.UserId, (statuses, achievements) =>
				{
					//за 10 использований сброса результатов
					achievements.SetAchievement(UserAchievement.BadLuck, () => message.Count >= Program.ReloadedSettings(model => model.BadLuckAchievementRetriesCount).Invoke());
				});
			}
		}
	}
}