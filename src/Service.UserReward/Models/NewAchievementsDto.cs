using System.Collections.Generic;
using Service.Core.Domain.Models.Constants;
using Service.Core.Domain.Models.Education;

namespace Service.UserReward.Models
{
	public class NewAchievementsDto
	{
		public NewAchievementsDto(EducationTutorial tutorial, int unit)
		{
			Tutorial = tutorial;
			Unit = unit;
		}

		public EducationTutorial Tutorial { get; set; }

		public int Unit { get; set; }

		public List<UserAchievement> Achievements { get; set; }
	}
}