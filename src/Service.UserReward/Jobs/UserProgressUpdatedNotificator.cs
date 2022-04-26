using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.Core.Client.Constants;
using Service.ServiceBus.Models;
using Service.UserReward.Helpers;
using Service.UserReward.Services;
using IDtoRepository = Service.UserReward.Services.IDtoRepository;

namespace Service.UserReward.Jobs
{
	public class UserProgressUpdatedNotificator : NotificatorBase
	{
		public UserProgressUpdatedNotificator(ILogger<UserProgressUpdatedNotificator> logger,
			ISubscriber<IReadOnlyList<UserProgressUpdatedServiceBusModel>> subscriber,
			IDtoRepository dtoRepository,
			ITotalRewardService totalRewardService, IServiceBusPublisher<UserRewardedServiceBusModel> publisher) 
			: base(dtoRepository, totalRewardService, logger, publisher) =>
				subscriber.Subscribe(HandleEvent);

		private async ValueTask HandleEvent(IReadOnlyList<UserProgressUpdatedServiceBusModel> events)
		{
			foreach (UserProgressUpdatedServiceBusModel message in events)
			{
				await Process(message.UserId, (statuses, achievements) =>
				{
					//за пройденные задачи определенного типа (шаг изменения = 2 за сформировавшиеся привычки)
					void AddStrategistStatus(int level) => statuses.SetStatus(UserStatus.Strategist, level);

					switch (message.HabitCount)
					{
						case 1:
							//освоил одну привычку
							achievements.SetAchievement(UserAchievement.Habitant);
							break;
						case 2:
							AddStrategistStatus(1);
							break;
						case 4:
							AddStrategistStatus(2);
							break;
						case 6:
							AddStrategistStatus(3);
							break;
						case 8:
							AddStrategistStatus(4);
							break;
						case 9:
							//освоил все привычки
							achievements.SetAchievement(UserAchievement.TheHabitMaster);
							AddStrategistStatus(5);
							break;
					}
				});
			}
		}
	}
}