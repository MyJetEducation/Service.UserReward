using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Constants;
using Service.Core.Client.Services;
using Service.ServiceBus.Models;
using Service.UserReward.Helpers;
using Service.UserReward.Services;

namespace Service.UserReward.Jobs
{
	public class MarketProductPurchasedNotificator : NotificatorBase
	{
		private readonly ISystemClock _systemClock;

		private readonly Func<int> _weeksCount = Program.ReloadedSettings(model => model.DoubleQuickWeekCount);

		public MarketProductPurchasedNotificator(ILogger<MarketProductPurchasedNotificator> logger,
			ISubscriber<IReadOnlyList<MarketProductPurchasedServiceBusModel>> subscriber,
			IDtoRepository dtoRepository,
			ITotalRewardService totalRewardService, ISystemClock systemClock) : base(dtoRepository, totalRewardService, logger)
		{
			_systemClock = systemClock;
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<MarketProductPurchasedServiceBusModel> events)
		{
			int weeksCount = _weeksCount.Invoke();

			foreach (MarketProductPurchasedServiceBusModel message in events)
			{
				string userId = message.UserId;

				DateTime[] dates = await GetPurchaseDates(userId, weeksCount, _systemClock.Today);

				await Process(userId, (statuses, achievements) =>
				{
					//������� �������������� ���������� ������� (��� ����������)
					achievements.SetAchievement(UserAchievement.SoEasy);

					//�������� ��� ������ �� ���� ������ ������
					achievements.SetAchievement(UserAchievement.Flash, () => message.ProductPrice > 0 && message.AccountValue == 0);

					//�������� ������� ������� 1 ��� � ������ 4 ������ ������
					achievements.SetAchievement(UserAchievement.DoubleQuick, () => AchievementHelper.CheckPurchaseDates(weeksCount, dates));
				});
			}
		}

		private async ValueTask<DateTime[]> GetPurchaseDates(string userId, int weeksCount, DateTime today)
		{
			DateTime[] dates = (await DtoRepository.GetPurchaseDates(userId))
				.Where(dt => today.Subtract(dt).TotalDays <= AchievementHelper.WeekDays * (weeksCount + 1) + 1)
				.Union(new[] {_systemClock.Today})
				.ToArray();

			bool setResponse = await DtoRepository.SetPurchaseDates(userId, dates);
			if (!setResponse)
			{
				Logger.LogError("Can't save DoubleQuick purchase dates for user: {user}, dates: {@dates}", userId, dates);

				return null;
			}

			return dates;
		}
	}
}