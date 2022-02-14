using System.Collections.Generic;
using System.Linq;
using Service.Core.Client.Constants;
using Service.UserReward.Grpc.Models;
using Service.UserReward.Models;

namespace Service.UserReward.Mappers
{
	public static class RewardMapper
	{
		public static UserAchievementsGrpcResponse ToGrpcModel(this IEnumerable<UserAchievement> achievements) => new UserAchievementsGrpcResponse
		{
			Items = achievements.ToArray()
		};

		public static UserStatusesGrpcResponse ToGrpcModel(this IEnumerable<StatusDto> statuses) => new UserStatusesGrpcResponse
		{
			Items = statuses.Select(dto => new StatusGrpcModel
			{
				Status = dto.Status,
				Level = dto.Level
			}).ToArray()
		};
	}
}