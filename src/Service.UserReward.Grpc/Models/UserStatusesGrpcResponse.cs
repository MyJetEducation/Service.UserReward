using System.Runtime.Serialization;

namespace Service.UserReward.Grpc.Models
{
	[DataContract]
	public class UserStatusesGrpcResponse
	{
		[DataMember(Order = 1)]
		public StatusGrpcModel[] Items { get; set; }
	}
}