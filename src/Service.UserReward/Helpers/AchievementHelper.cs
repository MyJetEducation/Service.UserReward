using System;
using System.Linq;
using Service.Core.Client.Constants;
using Service.UserReward.Constants;
using Service.UserReward.Models;

namespace Service.UserReward.Helpers
{
	public static class AchievementHelper
	{
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

		public static (UserAchievement achievement, AchievementType type)[] AchievementTypeInfo => new (UserAchievement achievement, AchievementType type)[]
		{
			//Standard
			(UserAchievement.Starter, AchievementType.Standard),
			(UserAchievement.Ignition, AchievementType.Standard),
			(UserAchievement.Voila, AchievementType.Standard),
			(UserAchievement.Welcome, AchievementType.Standard),
			(UserAchievement.FirstTouch, AchievementType.Standard),
			(UserAchievement.Eyescatter, AchievementType.Standard),
			(UserAchievement.PjFry, AchievementType.Standard),
			(UserAchievement.SoEasy, AchievementType.Standard),
			(UserAchievement.Habitant, AchievementType.Standard),
			(UserAchievement.GreenArrow, AchievementType.Standard),

			//Rare
			(UserAchievement.NowIKnow, AchievementType.Rare),
			(UserAchievement.TakeYourTime, AchievementType.Rare),
			(UserAchievement.ALongWay, AchievementType.Rare),
			(UserAchievement.IveSeenThis, AchievementType.Rare),
			(UserAchievement.Bender, AchievementType.Rare),
			(UserAchievement.TheSeeker, AchievementType.Rare),
			(UserAchievement.TheGreatChanger, AchievementType.Rare),
			(UserAchievement.WeGotCompany, AchievementType.Rare),
			(UserAchievement.Companion, AchievementType.Rare),
			(UserAchievement.NewBeginning, AchievementType.Rare),
			(UserAchievement.BadLuck, AchievementType.Rare),
			(UserAchievement.Unstoppable, AchievementType.Rare),
			(UserAchievement.Paradox, AchievementType.Rare),
			(UserAchievement.Spender, AchievementType.Rare),
			(UserAchievement.Trinity, AchievementType.Rare),

			//SuperRare
			(UserAchievement.Bullseye, AchievementType.SuperRare),
			(UserAchievement.Insister, AchievementType.SuperRare),
			(UserAchievement.PerfectTiming, AchievementType.SuperRare),
			(UserAchievement.DoubleQuick, AchievementType.SuperRare),
			(UserAchievement.RareCollector, AchievementType.SuperRare),
			(UserAchievement.Flash, AchievementType.SuperRare),
			(UserAchievement.YouCantHide, AchievementType.SuperRare),
			(UserAchievement.FaceTime, AchievementType.SuperRare),
			(UserAchievement.TheHabitMaster, AchievementType.SuperRare),
			(UserAchievement.Dwarf, AchievementType.SuperRare),
			(UserAchievement.Stability, AchievementType.SuperRare),

			//UltraRare
			(UserAchievement.CheckMe, AchievementType.UltraRare),
			(UserAchievement.SuperRareCollector, AchievementType.UltraRare),
			(UserAchievement.AllTheKingsMen, AchievementType.UltraRare),
			(UserAchievement.RoundTheWorld, AchievementType.UltraRare),
			(UserAchievement.DayByDay, AchievementType.UltraRare),
			(UserAchievement.InStore, AchievementType.UltraRare),

			//Standard
			(UserAchievement.UltraRareCollector, AchievementType.Unique),
			(UserAchievement.Curious, AchievementType.Unique),
			(UserAchievement.TaDam, AchievementType.Unique),
			(UserAchievement.NotSoHard, AchievementType.Unique),
			(UserAchievement.MyPride, AchievementType.Unique),
			(UserAchievement.Split, AchievementType.Unique),
			(UserAchievement.Complaisance, AchievementType.Unique)
		};

		public static bool HasAllAchievementByType(AchievementInfo achievements, AchievementType type) =>
			HasAchievement(achievements, achievementType => achievementType == type);

		public static bool HasAchievement(AchievementInfo achievements, Func<AchievementType, bool> func)
		{
			int userAchievementCount = GetAchievementCount(achievements, func);

			int allAchievementCount = AchievementTypeInfo.Count(tuple => func.Invoke(tuple.type));

			return userAchievementCount == allAchievementCount;
		}

		public static int GetAchievementCount(AchievementInfo achievements, Func<AchievementType, bool> func)
		{
			return (from achievement in achievements.Items
				join typeInfo in AchievementTypeInfo on achievement equals typeInfo.achievement
				where func.Invoke(typeInfo.type)
				select achievement)
				.Count();
		}
	}
}