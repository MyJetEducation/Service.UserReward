using MyYamlParser;

namespace Service.UserReward.Settings
{
	public class UserTimeRewardsSettingsModel
	{
		[YamlProperty("TakeYourTimeHours")]
		public int TakeYourTimeHours { get; set; }

		[YamlProperty("ALongWayDays")]
		public int ALongWayDays { get; set; }

		[YamlProperty("PerfectTimingDays")]
		public int PerfectTimingDays { get; set; }

		[YamlProperty("LongLiverDays")]
		public int LongLiverDays { get; set; }
	}
}