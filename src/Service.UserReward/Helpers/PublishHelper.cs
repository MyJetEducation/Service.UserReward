using System.Linq;
using System.Threading.Tasks;
using MyJetWallet.Sdk.ServiceBus;
using Service.ServiceBus.Models;
using Service.UserReward.Models;

namespace Service.UserReward.Helpers
{
	public static class PublishHelper
	{
		/// <summary>
		///     Publish new statuses and achievements for increase user tokens
		/// </summary>
		public static async ValueTask TryPublishUserRewarded(IServiceBusPublisher<UserRewardedServiceBusModel> publisher, string userId, StatusInfo statuses, AchievementInfo achievements)
		{
			if (statuses.Changed || achievements.Changed)
				await publisher.PublishAsync(new UserRewardedServiceBusModel
				{
					UserId = userId,
					Achievements = achievements.NewItems.ToArray(),
					Statuses = statuses.NewItems.Select(dto => new UserStatusGrpcModel
					{
						Status = dto.Status,
						Level = dto.Level
					}).ToArray()
				});
		}
	}
}