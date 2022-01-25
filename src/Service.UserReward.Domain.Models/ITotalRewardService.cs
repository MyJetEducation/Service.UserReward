using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Core.Domain.Models.Constants;
using Service.Core.Grpc.Models;

namespace Service.UserReward.Domain.Models
{
	public interface ITotalRewardService
	{
		ValueTask<CommonGrpcResponse> CheckTotal(Guid? userId, List<StatusDto> statuses, List<UserAchievement> achievements, bool statusesChanged, bool achievementsChanged);
	}
}