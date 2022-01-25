using MyYamlParser;

namespace Service.UserReward.Settings
{
	public class KeyRewardStatusCountSettingsModel
	{
		[YamlProperty("StandardCount")]
		public int StandardCount { get; set; }

		[YamlProperty("RareCount")]
		public int RareCount { get; set; }

		[YamlProperty("SuperRareCount")]
		public int SuperRareCount { get; set; }

		[YamlProperty("UltraRareCount")]
		public int UltraRareCount { get; set; }

		[YamlProperty("UniqueCount")]
		public int UniqueCount { get; set; }
	}
}