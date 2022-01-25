using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.Core.Domain.Models.Constants;
using Service.Core.Domain.Models.Education;
using Service.EducationProgress.Domain.Models;
using Service.EducationProgress.Grpc.ServiceBusModels;
using Service.UserReward.Domain.Models;
using Service.UserReward.Helpers;
using Service.UserReward.Settings;

namespace Service.UserReward.Services
{
	public class StatusRewardService : IStatusRewardService
	{
		private readonly IDtoRepository _dtoRepository;

		public StatusRewardService(IDtoRepository dtoRepository) => _dtoRepository = dtoRepository;

		public async Task<bool> CheckByProgress(SetProgressInfoServiceBusModel model, EducationProgressDto[] educationProgress, List<StatusDto> statuses)
		{
			Guid? userId = model.UserId;
			int unit = model.Unit;
			int task = model.Task;
			EducationTutorial tutorial = model.Tutorial;
			Dictionary<EducationTaskType, int> tasksByType = GetTasksByType(educationProgress);

			//выбрана дисциплина и пройден 1 урок
			bool changed = statuses.SetStatus(UserStatus.Newbie, () => tutorial == EducationTutorial.PersonalFinance && unit == 1 && task == 1);

			TaskFinishedStepCountSettingsModel taskFinishedStepCountSettings = Program.ReloadedSettings(sets => sets.TaskFinishedStepCount).Invoke();

			//за 9 пройденных тестов
			changed = changed || statuses.SetStatus(UserStatus.Analyst, tasksByType[EducationTaskType.Test] / taskFinishedStepCountSettings.TestCount);

			//за 9 пройденных видео
			changed = changed || statuses.SetStatus(UserStatus.Financier, tasksByType[EducationTaskType.Video] / taskFinishedStepCountSettings.VideoCount);

			//за 9 прочтенных текстов
			changed = changed || statuses.SetStatus(UserStatus.Investor, tasksByType[EducationTaskType.Text] / taskFinishedStepCountSettings.TextCount);

			//изучено 5 дисциплин полностью
			changed = changed || statuses.SetStatus(UserStatus.Bachelor, () => IsTutorialLearned(educationProgress, EducationTutorial.HealthAndFinance));

			//изучены все дисциплины
			changed = changed || statuses.SetStatus(UserStatus.Magister, () => IsTutorialLearned(educationProgress, EducationTutorial.Economics));

			//за выполнение задач Test на 100% с 1 попытки
			if (!statuses.Any(dto => dto.Status == UserStatus.Expert && dto.Level == 5))
			{
				TestTasks100PrcDto tasks100Prc = await _dtoRepository.GetTestTasks100Prc(userId);
				if (tasks100Prc.Count == 9)
				{
					int maxLevel = statuses
						.Where(dto => dto.Status == UserStatus.Expert)
						.Max(dto => dto.Level)
						.GetValueOrDefault();

					changed = changed || statuses.SetStatus(UserStatus.Expert, () => maxLevel < 5, maxLevel + 1);

					await _dtoRepository.ClearTestTasks100Prc(userId);
				}
			}

			return changed;
		}

		private static bool IsTutorialLearned(EducationProgressDto[] educationProgress, EducationTutorial tutorial) =>
			educationProgress.Where(dto => dto.Tutorial == tutorial).Average(dto => dto.Value.GetValueOrDefault()) >= AnswerProgress.OkAnswerProgress;

		private static Dictionary<EducationTaskType, int> GetTasksByType(EducationProgressDto[] educationProgress) => educationProgress
			.GroupBy(dto => EducationHelper.GetTask(dto.Tutorial, dto.Unit, dto.Task).TaskType)
			.ToDictionary(grouping => grouping.Key, grouping =>
				grouping.Count(arg => arg.IsOk()));

		public bool CheckTotal(List<StatusDto> statuses, IEnumerable<UserAchievement> achievements)
		{
			RewardStatusCountSettingsModel rewardStatusCountSettings = Program.ReloadedSettings(model => model.RewardStatusCount).Invoke();

			//за опредленное количество ачивок каждого типа (1-3-5)

			bool changed = statuses.SetStatus(UserStatus.Rewarded, () => rewardStatusCountSettings.StandardCount > 0
				&& AchievementHelper.GetAchievementCount(achievements, type => type == AchievementType.Standard) >= rewardStatusCountSettings.StandardCount)

			|| statuses.SetStatus(UserStatus.Rewarded, () => rewardStatusCountSettings.RareCount > 0
				&& AchievementHelper.GetAchievementCount(achievements, type => type == AchievementType.Rare) >= rewardStatusCountSettings.RareCount, 2)

			|| statuses.SetStatus(UserStatus.Rewarded, () => rewardStatusCountSettings.SuperRareCount> 0
				&& AchievementHelper.GetAchievementCount(achievements, type => type == AchievementType.SuperRare) >= rewardStatusCountSettings.SuperRareCount, 3)

			|| statuses.SetStatus(UserStatus.Rewarded, () => rewardStatusCountSettings.UltraRareCount > 0
				&& AchievementHelper.GetAchievementCount(achievements, type => type == AchievementType.UltraRare) >= rewardStatusCountSettings.UltraRareCount, 4)

			|| statuses.SetStatus(UserStatus.Rewarded, () => rewardStatusCountSettings.UniqueCount > 0
				&& AchievementHelper.GetAchievementCount(achievements, type => type == AchievementType.Unique) >= rewardStatusCountSettings.UniqueCount, 5);

			return changed;
		}
	}
}