using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.Core.Client.Constants;
using Service.Education.Constants;
using Service.Education.Helpers;
using Service.Education.Structure;
using Service.EducationProgress.Domain.Models;
using Service.ServiceBus.Models;
using Service.UserReward.Constants;
using Service.UserReward.Helpers;
using Service.UserReward.Models;

namespace Service.UserReward.Services
{
	public class StatusRewardService : IStatusRewardService
	{
		private readonly IDtoRepository _dtoRepository;

		public StatusRewardService(IDtoRepository dtoRepository) => _dtoRepository = dtoRepository;

		public async ValueTask CheckByProgress(SetProgressInfoServiceBusModel model, EducationProgressDto[] educationProgress, StatusInfo statuses)
		{
			Guid? userId = model.UserId;
			int unit = model.Unit;
			int task = model.Task;
			EducationTutorial tutorial = model.Tutorial;
			Dictionary<EducationTaskType, int> tasksByType = GetTasksByType(educationProgress);

			int FinishedTaskByType(EducationTaskType type, Func<int> settingsCountFunc) => 
				!tasksByType.ContainsKey(type) ? 0 : tasksByType[type] / settingsCountFunc.Invoke();

			//выбрана дисциплина и пройден 1 урок
			statuses.SetStatus(UserStatus.Newbie, () => tutorial == EducationTutorial.PersonalFinance && unit == 1 && task == 1)

			//за 9 пройденных тестов
			.SetStatus(UserStatus.Analyst, FinishedTaskByType(EducationTaskType.Test, Program.ReloadedSettings(sets => sets.TaskFinishedStepCount.TestCount)))

			//за 9 пройденных видео
			.SetStatus(UserStatus.Financier, FinishedTaskByType(EducationTaskType.Video, Program.ReloadedSettings(sets => sets.TaskFinishedStepCount.VideoCount)))

			//за 9 прочтенных текстов
			.SetStatus(UserStatus.Investor, FinishedTaskByType(EducationTaskType.Text, Program.ReloadedSettings(sets => sets.TaskFinishedStepCount.TextCount)))

			//изучено 5 дисциплин полностью
			.SetStatus(UserStatus.Bachelor, () => IsTutorialLearned(educationProgress, EducationTutorial.HealthAndFinance))

			//изучены все дисциплины
			.SetStatus(UserStatus.Magister, () => IsTutorialLearned(educationProgress, EducationTutorial.Economics));

			//за выполнение задач Test на 100% с 1 попытки
			if (!statuses.Items.Any(dto => dto.Status == UserStatus.Expert && dto.Level == 5))
			{
				TestTasks100PrcDto tasks100Prc = await _dtoRepository.GetTestTasks100Prc(userId);
				if (tasks100Prc.Count == 9)
				{
					int? expertLevel = statuses.Items.FirstOrDefault(dto => dto.Status == UserStatus.Expert)?.Level;

					statuses.SetStatus(UserStatus.Expert, expertLevel.GetValueOrDefault() + 1);

					await _dtoRepository.ClearTestTasks100Prc(userId);
				}
			}
		}

		private static bool IsTutorialLearned(IEnumerable<EducationProgressDto> educationProgress, EducationTutorial tutorial)
		{
			EducationProgressDto[] educationProgressDtos = educationProgress.Where(dto => dto.Tutorial == tutorial).ToArray();

			return educationProgressDtos.Any() && educationProgressDtos.Average(dto => dto.Value.GetValueOrDefault()) >= Progress.OkProgress;
		}

		private static Dictionary<EducationTaskType, int> GetTasksByType(IEnumerable<EducationProgressDto> educationProgress) => educationProgress
			.GroupBy(dto => EducationHelper.GetTask(dto.Tutorial, dto.Unit, dto.Task).TaskType)
			.ToDictionary(grouping => grouping.Key, grouping =>
				grouping.Count(arg => arg.IsOk()));

		public void CheckTotal(StatusInfo statuses, AchievementInfo achievements)
		{
			void TrySetRewarded(Func<int> defaultCountFunc, int level, AchievementType achievementType)
			{
				if (statuses.Items.Any(dto => dto.Status == UserStatus.Rewarded && dto.Level == level))
					return;

				int defaultCount = defaultCountFunc.Invoke();
				if (defaultCount == 0)
					return;
				
				statuses.SetStatus(UserStatus.Rewarded, () => AchievementHelper.GetAchievementCount(achievements, type => type == achievementType) >= defaultCount, level);
			}

			//за опредленное количество ачивок каждого типа (1-3-5)
			TrySetRewarded(Program.ReloadedSettings(model => model.RewardStatusCount.StandardCount), 1, AchievementType.Standard);
			TrySetRewarded(Program.ReloadedSettings(model => model.RewardStatusCount.RareCount), 2, AchievementType.Rare);
			TrySetRewarded(Program.ReloadedSettings(model => model.RewardStatusCount.SuperRareCount), 3, AchievementType.SuperRare);
			TrySetRewarded(Program.ReloadedSettings(model => model.RewardStatusCount.UltraRareCount), 4, AchievementType.UltraRare);
			TrySetRewarded(Program.ReloadedSettings(model => model.RewardStatusCount.UniqueCount), 5, AchievementType.Unique);
		}
	}
}