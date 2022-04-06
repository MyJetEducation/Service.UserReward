using System.Runtime.Serialization;

namespace Service.UserReward.Grpc.Models
{
	[DataContract]
	public class GetUserAchievementsGrpcRequest
	{
		[DataMember(Order = 1)]
		public string UserId { get; set; }

		[DataMember(Order = 2)]
		public int? Unit { get; set; }
	}
}