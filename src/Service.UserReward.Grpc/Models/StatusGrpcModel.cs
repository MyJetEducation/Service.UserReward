using System.Runtime.Serialization;
using Service.Core.Domain.Models.Constants;

namespace Service.UserReward.Grpc.Models
{
	[DataContract]
	public class StatusGrpcModel
	{
		[DataMember(Order = 1)]
		public UserStatus Status { get; set; }

		[DataMember(Order = 2)]
		public int? Level { get; set; }
	}
}