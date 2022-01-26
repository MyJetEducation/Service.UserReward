using System.Collections.Generic;
using Service.Core.Domain.Extensions;
using Service.Core.Domain.Models.Constants;

namespace Service.UserReward.Models
{
	public class AchievementInfo
	{
		public AchievementInfo(List<UserAchievement> items)
		{
			Items = items;
			NewItems = new List<UserAchievement>();
		}

		public List<UserAchievement> Items { get; }

		public List<UserAchievement> NewItems { get; }

		public void AddItem(UserAchievement achievement)
		{
			Items.Add(achievement);
			NewItems.Add(achievement);
		}

		public bool Changed => !NewItems.IsNullOrEmpty();
	}
}