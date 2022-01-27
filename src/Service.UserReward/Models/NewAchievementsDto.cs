using System.Collections.Generic;
using Service.Core.Client.Constants;
using Service.Core.Client.Education;

namespace Service.UserReward.Models
{
	public class NewAchievementsDto
	{
		public NewAchievementsDto(EducationTutorial tutorial, int unit)
		{
			Tutorial = tutorial;
			Unit = unit;
			Achievements = new List<UserAchievement>();
		}

		public EducationTutorial Tutorial { get; set; }

		public int Unit { get; set; }

		public List<UserAchievement> Achievements { get; set; }
	}
}