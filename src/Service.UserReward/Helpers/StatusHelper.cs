using System;
using System.Linq;
using Service.Core.Client.Constants;
using Service.UserReward.Models;

namespace Service.UserReward.Helpers
{
	public static class StatusHelper
	{
		public static StatusInfo SetStatus(this StatusInfo info, UserStatus status, Func<bool> predicate, int level = 1)
		{
			if (info.Items.Any(dto => dto.Status == status && dto.Level == level) || !predicate.Invoke())
				return info;

			info.Items.Add(new StatusDto(status, level));
			info.Changed = true;

			return info;
		}

		public static StatusInfo SetStatus(this StatusInfo info, UserStatus status, int level = 1)
		{
			if (level == 0 || level > 5 || info.Items.Any(dto => dto.Status == status && dto.Level == level))
				return info;

			info.Items.Add(new StatusDto(status, level));
			info.Changed = true;

			return info;
		}
	}
}