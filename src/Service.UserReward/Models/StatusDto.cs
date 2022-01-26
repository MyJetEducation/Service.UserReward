using Service.Core.Domain.Models.Constants;

namespace Service.UserReward.Models
{
	public class StatusDto
	{
		public UserStatus Status { get; set; }

		public int? Level { get; set; }
	}
}