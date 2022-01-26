using System.Collections.Generic;

namespace Service.UserReward.Models
{
	public class StatusInfo
	{
		public StatusInfo(List<StatusDto> items) => Items = items;

		public List<StatusDto> Items { get; set; }

		public bool Changed { get; set; }
	}
}