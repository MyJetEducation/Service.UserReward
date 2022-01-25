using System.Collections.Generic;
using System.Linq;
using Service.Core.Domain.Models.Constants;
using Service.UserReward.Domain.Models;
using Service.UserReward.Grpc.Models;

namespace Service.UserReward.Mappers
{
	public static class RewardMapper
	{
		public static UserAchievementsGrpcResponse ToGrpcModel(this IEnumerable<UserAchievement> achievements) => new UserAchievementsGrpcResponse
		{
			Items = achievements.ToArray()
		};

		public static UserAchievementsGrpcResponse ToGrpcModel(this NewAchievementsDto newAchievements) => new UserAchievementsGrpcResponse
		{
			Items = newAchievements.Achievements.ToArray()
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