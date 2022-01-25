using Service.Core.Domain.Models.Constants;
using Service.Core.Domain.Models.Education;

namespace Service.UserReward.Domain.Models
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

		public UserAchievement[] Achievements { get; set; }
	}
}