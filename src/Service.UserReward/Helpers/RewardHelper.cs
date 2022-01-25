using System;
using System.Collections.Generic;
using System.Linq;
using Service.Core.Domain.Models.Constants;
using Service.EducationProgress.Domain.Models;
using Service.UserReward.Domain.Models;

namespace Service.UserReward.Helpers
{
	public static class RewardHelper
	{
		public static bool SetAchievement(this List<UserAchievement> achievementList, UserAchievement achievement, Func<bool> func)
		{
			if (achievementList.Contains(achievement) || !func.Invoke())
				return false;

			achievementList.Add(achievement);

			return true;
		}

		public static bool SetAchievement(this List<UserAchievement> achievementList, UserAchievement achievement)
		{
			if (achievementList.Contains(achievement))
				return false;

			achievementList.Add(achievement);

			return true;
		}

		public static bool SetStatus(this List<StatusDto> statusList, UserStatus status, Func<bool> func, int level = 1)
		{
			if (statusList.Any(dto => dto.Status == status && dto.Level == level) || !func.Invoke())
				return false;

			statusList.Add(new StatusDto
			{
				Status = status,
				Level = level
			});

			return true;
		}

		public static bool SetStatus(this List<StatusDto> statusList, UserStatus status, int level = 1)
		{
			if (level == 0 || level > 5 || statusList.Any(dto => dto.Status == status && dto.Level == level))
				return false;

			statusList.Add(new StatusDto
			{
				Status = status,
				Level = level
			});

			return true;
		}

		public static bool IsOk(this EducationProgressDto dto) => dto.Value >= AnswerProgress.OkAnswerProgress;

		public static bool IsMax(this EducationProgressDto dto) => dto.Value == AnswerProgress.MaxAnswerProgress;

		public static bool IsMin(this EducationProgressDto dto) => dto.Value == AnswerProgress.MinAnswerProgress;
	}
}