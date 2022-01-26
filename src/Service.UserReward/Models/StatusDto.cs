using Service.Core.Client.Constants;

namespace Service.UserReward.Models
{
	public class StatusDto
	{
		public UserStatus Status { get; set; }

		public int? Level { get; set; }
	}
}