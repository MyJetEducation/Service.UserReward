using Service.Core.Domain.Models.Constants;

namespace Service.UserReward.Domain.Models
{
	public class StatusDto
	{
		public UserStatus Status { get; set; }

		public int? Level { get; set; }
	}
}