using System;
using System.Linq;
using Service.Core.Client.Constants;
using Service.Core.Client.Extensions;
using Service.UserReward.Domain;
using Service.UserReward.Models;

namespace Service.UserReward.Helpers
{
	public static class AchievementHelper
	{
		public const int WeekDays = 7;

		public static AchievementInfo SetAchievement(this AchievementInfo info, UserAchievement achievement, Func<bool> predicate)
		{
			if (info.Items.Contains(achievement) || !predicate.Invoke())
				return info;

			info.AddItem(achievement);

			return info;
		}

		public static AchievementInfo SetAchievement(this AchievementInfo info, UserAchievement achievement)
		{
			if (info.Items.Contains(achievement))
				return info;

			info.AddItem(achievement);

			return info;
		}

		public static bool HasAllAchievementByType(AchievementInfo achievements, AchievementType type) =>
			HasAchievement(achievements, achievementType => achievementType == type);

		public static bool HasAchievement(AchievementInfo achievements, Func<AchievementType, bool> func)
		{
			int userAchievementCount = GetAchievementCount(achievements, func);

			int allAchievementCount = AchievementTypeHelper.AchievementTypeInfo.Count(tuple => func.Invoke(tuple.type));

			return userAchievementCount == allAchievementCount;
		}

		public static int GetAchievementCount(AchievementInfo achievements, Func<AchievementType, bool> func)
		{
			return (from achievement in achievements.Items
				join typeInfo in AchievementTypeHelper.AchievementTypeInfo on achievement equals typeInfo.achievement
				where func.Invoke(typeInfo.type)
				select achievement)
				.Count();
		}

		public static bool CheckPurchaseDates(int weeksCount, DateTime[] dates)
		{
			if (dates.IsNullOrEmpty())
				return false;

			DateTime today = dates.Last();

			for (var dispDay = 0; dispDay <= WeekDays - 1; dispDay++)
				if (Enumerable.Range(1, weeksCount)
					.All(weekNum => dates.Any(dt => today.AddDays(-1 * WeekDays * weekNum + dispDay) <= dt && dt <= today.AddDays(-1 * WeekDays * (weekNum - 1) + dispDay))))
					return true;

			return false;
		}
	}
}