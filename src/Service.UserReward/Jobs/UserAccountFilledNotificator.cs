using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
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
			ITotalRewardService totalRewardService) : base(dtoRepository, totalRewardService, logger) =>
				subscriber.Subscribe(HandleEvent);

		private async ValueTask HandleEvent(IReadOnlyList<UserAccountFilledServiceBusModel> events)
		{
			foreach (UserAccountFilledServiceBusModel message in events)
			{
				await Process(message.UserId, (statuses, achievements) =>
				{
					//���� ��� ������������ ������ � �������
					achievements.SetAchievement(UserAchievement.Complaisance);
				});
			}
		}
	}
}