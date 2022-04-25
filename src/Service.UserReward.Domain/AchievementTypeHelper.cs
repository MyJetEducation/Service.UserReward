using System.Linq;
using Service.Core.Client.Constants;

namespace Service.UserReward.Domain
{
	public class AchievementTypeHelper
	{
		public static AchievementType GetAchievementType(UserAchievement achievement) => AchievementTypeInfo.First(tuple => tuple.achievement == achievement).type;

		public static (UserAchievement achievement, AchievementType type)[] AchievementTypeInfo => new (UserAchievement achievement, AchievementType type)[]
		{
			//Standard
			(UserAchievement.Ignition, AchievementType.Standard),
			(UserAchievement.Voila, AchievementType.Standard),
			(UserAchievement.Eyescatter, AchievementType.Standard),
			(UserAchievement.SoEasy, AchievementType.Standard),
			(UserAchievement.Habitant, AchievementType.Standard),
			(UserAchievement.GreenArrow, AchievementType.Standard),
			(UserAchievement.Complaisance, AchievementType.Standard),

			//Rare
			(UserAchievement.NowIKnow, AchievementType.Rare),
			(UserAchievement.TakeYourTime, AchievementType.Rare),
			(UserAchievement.ALongWay, AchievementType.Rare),
			(UserAchievement.IveSeenThis, AchievementType.Rare),
			(UserAchievement.TheSeeker, AchievementType.Rare),
			(UserAchievement.BadLuck, AchievementType.Rare),
			(UserAchievement.Unstoppable, AchievementType.Rare),
			(UserAchievement.Paradox, AchievementType.Rare),
			(UserAchievement.Trinity, AchievementType.Rare),

			//SuperRare
			(UserAchievement.Bullseye, AchievementType.SuperRare),
			(UserAchievement.Insister, AchievementType.SuperRare),
			(UserAchievement.PerfectTiming, AchievementType.SuperRare),
			(UserAchievement.DoubleQuick, AchievementType.SuperRare),
			(UserAchievement.RareCollector, AchievementType.SuperRare),
			(UserAchievement.Flash, AchievementType.SuperRare),
			(UserAchievement.TheHabitMaster, AchievementType.SuperRare),
			(UserAchievement.Stability, AchievementType.SuperRare),

			//UltraRare
			(UserAchievement.CheckMe, AchievementType.UltraRare),
			(UserAchievement.SuperRareCollector, AchievementType.UltraRare),
			(UserAchievement.RoundTheWorld, AchievementType.UltraRare),

			//Standard
			(UserAchievement.UltraRareCollector, AchievementType.Unique),
			(UserAchievement.Curious, AchievementType.Unique),
			(UserAchievement.TaDam, AchievementType.Unique),
			(UserAchievement.NotSoHard, AchievementType.Unique),
			(UserAchievement.Split, AchievementType.Unique)
		};
	}
}