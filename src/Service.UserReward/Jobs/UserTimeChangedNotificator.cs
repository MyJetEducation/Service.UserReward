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
					//провел N времени в сервисе в течение дня
					achievements.SetAchievement(UserAchievement.TakeYourTime, () => message.TodaySpan.TotalHours >= settings.TakeYourTimeHours);

					//провел N времени в сервисе совокупно
					achievements.SetAchievement(UserAchievement.ALongWay, () => totalDays >= settings.ALongWayDays);

					//провел в сервисе нужное количество времени
					achievements.SetAchievement(UserAchievement.PerfectTiming, () => totalDays >= settings.PerfectTimingDays);

					//провел в сервисе 365 дней
					achievements.SetAchievement(UserAchievement.RoundTheWorld, () => totalDays >= 365);

					//за пребывание в сервисе в течение определенного времени. 3 месяца
					statuses.SetStatus(UserStatus.LongLiver, () => totalDays >= settings.LongLiverDays);
				});
			}
		}
	}
}