using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Constants;
using Service.ServiceBus.Models;
using Service.UserReward.Helpers;
using Service.UserReward.Services;
using Service.UserReward.Settings;

namespace Service.UserReward.Jobs
{
	public class UserTimeChangedNotificator : NotificatorBase
	{
		public UserTimeChangedNotificator(ILogger<UserTimeChangedNotificator> logger,
			ISubscriber<IReadOnlyList<UserTimeChangedServiceBusModel>> subscriber,
			IDtoRepository dtoRepository,
			ITotalRewardService totalRewardService) : base(dtoRepository, totalRewardService, logger) =>
				subscriber.Subscribe(HandleEvent);

		private async ValueTask HandleEvent(IReadOnlyList<UserTimeChangedServiceBusModel> events)
		{
			UserTimeRewardsSettingsModel settings = Program.ReloadedSettings(model => model.UserTimeRewards).Invoke();

			foreach (UserTimeChangedServiceBusModel message in events)
			{
				double totalDays = message.TotalSpan.TotalDays;

				await Process(message.UserId, (statuses, achievements) =>
				{
					//������ N ������� � ������� � ������� ���
					achievements.SetAchievement(UserAchievement.TakeYourTime, () => message.TodaySpan.TotalHours >= settings.TakeYourTimeHours);

					//������ N ������� � ������� ���������
					achievements.SetAchievement(UserAchievement.ALongWay, () => totalDays >= settings.ALongWayDays);

					//������ � ������� ������ ���������� �������
					achievements.SetAchievement(UserAchievement.PerfectTiming, () => totalDays >= settings.PerfectTimingDays);

					//������ � ������� 365 ����
					achievements.SetAchievement(UserAchievement.RoundTheWorld, () => totalDays >= 365);

					//�� ���������� � ������� � ������� ������������� �������. 3 ������
					statuses.SetStatus(UserStatus.LongLiver, () => totalDays >= settings.LongLiverDays);
				});
			}
		}
	}
}