using System.Collections.Generic;
using Service.Core.Client.Constants;
using Service.Core.Client.Extensions;

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