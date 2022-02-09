using MyYamlParser;
using Service.Education.Structure;

namespace Service.UserReward.Settings
{
	public class SecondsForTaskSettingsModel
	{
		[YamlProperty("ForCase")]
		public int ForCase { get; set; }

		[YamlProperty("ForTrueFalse")]
		public int ForTrueFalse { get; set; }

		[YamlProperty("ForTest")]
		public int ForTest { get; set; }

		[YamlProperty("ForGame")]
		public int ForGame { get; set; }

		[YamlProperty("ForText")]
		public int ForText { get; set; }

		[YamlProperty("ForVideo")]
		public int ForVideo { get; set; }

		public static int GetDefaultSeconds(EducationTaskType taskType)
		{
			SecondsForTaskSettingsModel settings = Program.ReloadedSettings(settingsModel => settingsModel.SecondsForTask).Invoke();
			return taskType switch {
				EducationTaskType.Case => settings.ForCase, 
				EducationTaskType.TrueFalse => settings.ForTrueFalse, 
				EducationTaskType.Test => settings.ForTest, 
				EducationTaskType.Game => settings.ForGame, 
				EducationTaskType.Text => settings.ForText, 
				EducationTaskType.Video => settings.ForVideo, 
				_ => int.MaxValue};
		}
	}
}