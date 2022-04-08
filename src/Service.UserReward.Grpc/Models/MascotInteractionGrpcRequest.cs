using System.Runtime.Serialization;

namespace Service.UserReward.Grpc.Models
{
	[DataContract]
	public class MascotInteractionGrpcRequest
	{
		[DataMember(Order = 1)]
		public string UserId { get; set; }
	}
}