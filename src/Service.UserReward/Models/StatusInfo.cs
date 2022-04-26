using System.Collections.Generic;
using System.Linq;
using Service.Core.Client.Constants;
using Service.Core.Client.Extensions;

namespace Service.UserReward.Models
{
	public class StatusInfo
	{
		public StatusInfo(List<StatusDto> items)
		{
			Items = items;
			NewItems = new List<StatusDto>();
		}

		public List<StatusDto> Items { get; }

		public List<StatusDto> NewItems { get; }

		public void AddItem(StatusDto status)
		{
			Items.Add(status);
			NewItems.Add(status);
		}

		public bool Changed => !NewItems.IsNullOrEmpty();

		public bool Exists(UserStatus status, int? level) => Items.Any(dto => dto.Status == status && dto.Level == level);
	}
}