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
	public class UserAccountFilledNotificator : NotificatorBase
	{
		public UserAccountFilledNotificator(ILogger<UserAccountFilledNotificator> logger,
			ISubscriber<IReadOnlyList<UserAccountFilledServiceBusModel>> subscriber,
			IDtoRepository dtoRepository,
			ITotalRewardService totalRewardService, IServiceBusPublisher<UserRewardedServiceBusModel> publisher) 
			: base(dtoRepository, totalRewardService, logger, publisher) =>
				subscriber.Subscribe(HandleEvent);

		private async ValueTask HandleEvent(IReadOnlyList<UserAccountFilledServiceBusModel> events)
		{
			foreach (UserAccountFilledServiceBusModel message in events)
			{
				await Process(message.UserId, (statuses, achievements) =>
				{
					//внес все персональные данные в профиле
					achievements.SetAchievement(UserAchievement.Complaisance);
				});
			}
		}
	}
}