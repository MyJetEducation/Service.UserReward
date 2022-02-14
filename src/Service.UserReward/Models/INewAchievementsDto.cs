using System.Collections.Generic;
using Service.Core.Client.Constants;

namespace Service.UserReward.Models
{
	public interface INewAchievementsDto
	{
		List<UserAchievement> Achievements { get; set; }
	}
}