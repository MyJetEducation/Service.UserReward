using Service.Core.Client.Constants;

namespace Service.UserReward.Models
{
	public class StatusDto
	{
		public StatusDto(UserStatus status, int? level = null)
		{
			Status = status;
			Level = level;
		}

		public UserStatus Status { get; set; }

		public int? Level { get; set; }
	}
}