using System;
using System.Runtime.Serialization;

namespace Service.UserReward.Grpc.ServiceBusModels
{
	/// <summary>
	///     Использование сброса результатов
	/// </summary>
	[DataContract]
	public class RetryUsedServiceBusModel
	{
		public const string TopicName = "myjeteducation-retry-used";

		[DataMember(Order = 1)]
		public Guid? UserId { get; set; }

		[DataMember(Order = 2)]
		public int TotalCount { get; set; }
	}
}