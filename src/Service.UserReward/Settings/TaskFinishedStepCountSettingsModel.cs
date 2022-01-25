using MyYamlParser;

namespace Service.UserReward.Settings
{
	public class TaskFinishedStepCountSettingsModel
	{
		[YamlProperty("TestCount")]
		public int TestCount { get; set; }

		[YamlProperty("VideoCount")]
		public int VideoCount { get; set; }

		[YamlProperty("TextCount")]
		public int TextCount { get; set; }
	}
}