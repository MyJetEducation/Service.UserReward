using System.Collections.Generic;
using Service.Core.Client.Constants;
using Service.Education.Structure;

namespace Service.UserReward.Models
{
	public class NewAchievementsTutorialDto : INewAchievementsDto
	{
		public NewAchievementsTutorialDto(EducationTutorial tutorial)
		{
			Tutorial = tutorial;
			Achievements = new List<UserAchievement>();
		}

		public EducationTutorial Tutorial { get; set; }

		public List<UserAchievement> Achievements { get; set; }
	}
}