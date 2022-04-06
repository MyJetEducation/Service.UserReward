using System.Runtime.Serialization;

namespace Service.UserReward.Grpc.Models
{
	[DataContract]
	public class GetUserStatusesGrpcRequest
	{
		[DataMember(Order = 1)]
		public string UserId { get; set; }
	}
}