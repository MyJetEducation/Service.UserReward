using System.Runtime.Serialization;
using Service.Core.Client.Constants;

namespace Service.UserReward.Grpc.Models
{
	[DataContract]
	public class UserAchievementsGrpcResponse
	{
		[DataMember(Order = 1)]
		public UserAchievement[] Items { get; set; }
	}
}