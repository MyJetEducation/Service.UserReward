using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.UserReward.Settings
{
	public class SettingsModel
	{
		[YamlProperty("UserReward.SeqServiceUrl")]
		public string SeqServiceUrl { get; set; }

		[YamlProperty("UserReward.ZipkinUrl")]
		public string ZipkinUrl { get; set; }

		[YamlProperty("UserReward.ElkLogs")]
		public LogElkSettings ElkLogs { get; set; }

		[YamlProperty("UserReward.ServiceBusReader")]
		public string ServiceBusReader { get; set; }

		[YamlProperty("UserReward.ServerKeyValueServiceUrl")]
		public string ServerKeyValueServiceUrl { get; set; }

		[YamlProperty("UserReward.EducationProgressServiceUrl")]
		public string EducationProgressServiceUrl { get; set; }

		[YamlProperty("UserReward.KeyUserStatus")]
		public string KeyUserStatus { get; set; }

		[YamlProperty("UserReward.KeyUserAchievement")]
		public string KeyUserAchievement { get; set; }

		[YamlProperty("UserReward.KeyUserNewAchievementUnit")]
		public string KeyUserNewAchievementUnit { get; set; }

		[YamlProperty("UserReward.KeyUserNewAchievementTutorial")]
		public string KeyUserNewAchievementTutorial { get; set; }

		[YamlProperty("UserReward.KeyTestTasks100Prc")]
		public string KeyTestTasks100Prc { get; set; }

		[YamlProperty("UserReward.KeyPurchaseDates")]
		public string KeyPurchaseDates { get; set; }

		[YamlProperty("UserReward.DoubleQuickWeekCount")]
		public int DoubleQuickWeekCount { get; set; }

		[YamlProperty("UserReward.BadLuckAchievementRetriesCount")]
		public int BadLuckAchievementRetriesCount { get; set; }

		[YamlProperty("UserReward.SecondsForTasks")]
		public SecondsForTaskSettingsModel SecondsForTask { get; set; }
		
		[YamlProperty("UserReward.RewardStatusCount")]
		public RewardStatusCountSettingsModel RewardStatusCount { get; set; }
		
		[YamlProperty("UserReward.TaskFinishedStepCount")]
		public TaskFinishedStepCountSettingsModel TaskFinishedStepCount { get; set; }
		
		[YamlProperty("UserReward.UserTimeRewards")]
		public UserTimeRewardsSettingsModel UserTimeRewards { get; set; }
	}
}