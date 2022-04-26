using System;
using Service.Core.Client.Constants;
using Service.UserReward.Models;

namespace Service.UserReward.Helpers
{
	public static class StatusHelper
	{
		public static StatusInfo SetStatus(this StatusInfo info, UserStatus status, Func<bool> predicate, int level = 1)
		{
			if (info.Exists(status, level) || !predicate.Invoke())
				return info;

			info.AddItem(new StatusDto(status, level));

			return info;
		}

		public static StatusInfo SetStatus(this StatusInfo info, UserStatus status, int level = 1)
		{
			if (level == 0 || level > 5 || info.Exists(status, level))
				return info;

			info.AddItem(new StatusDto(status, level));

			return info;
		}
	}
}