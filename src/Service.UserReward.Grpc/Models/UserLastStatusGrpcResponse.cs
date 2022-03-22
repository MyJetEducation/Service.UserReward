using System.Runtime.Serialization;

namespace Service.UserReward.Grpc.Models
{
	[DataContract]
	public class UserLastStatusGrpcResponse
	{
		[DataMember(Order = 1)]
		public StatusGrpcModel Status { get; set; }
	}
}