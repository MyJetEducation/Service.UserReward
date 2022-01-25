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

		[YamlProperty("UserReward.KeyUserStatus")]
		public string KeyUserStatus { get; set; }

		[YamlProperty("UserReward.KeyUserAchievement")]
		public string KeyUserAchievement { get; set; }

		[YamlProperty("UserReward.KeyUserNewAchievement")]
		public string KeyUserNewAchievement { get; set; }

		[YamlProperty("UserReward.KeyInvitedFriends")]
		public string KeyInvitedFriends { get; set; }

		[YamlProperty("UserReward.KeyEducationProgress")]
		public string KeyEducationProgress { get; set; }

		[YamlProperty("UserReward.KeyTestTasks100Prc")]
		public string KeyTestTasks100Prc { get; set; }

		[YamlProperty("UserReward.KeySecondsForTasks")]
		public KeySecondsForTaskSettingsModel KeySecondsForTask { get; set; }
		
		[YamlProperty("UserReward.KeyRewardStatusCount")]
		public KeyRewardStatusCountSettingsModel KeyRewardStatusCount { get; set; }
	}
}