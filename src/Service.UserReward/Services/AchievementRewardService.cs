using System;
using System.Collections.Generic;
using System.Linq;
using Service.Core.Client.Constants;
using Service.Core.Client.Extensions;
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
	public class AchievementRewardService : IAchievementRewardService
	{
		private static readonly (UserStatus status, int maxLevel)[] LeveledUserStatus =
		{
			(UserStatus.Expert, 5),
			(UserStatus.Analyst, 5),
			(UserStatus.Strategist, 5),
			(UserStatus.Financier, 5),
			(UserStatus.Investor, 5),
			(UserStatus.Rewarded, 3)
		};

		public void CheckByProgress(SetProgressInfoServiceBusModel model, EducationProgressDto[] educationProgress, AchievementInfo achievements)
		{
			int unit = model.Unit;
			int task = model.Task;
			EducationTutorial tutorial = model.Tutorial;

			EducationStructureTask structureTask = EducationHelper.GetTask(tutorial, unit, task);
			EducationTaskType taskType = structureTask.TaskType;
			EducationStructureTutorial structureTutorial = EducationStructure.Tutorials[tutorial];
			EducationStructureUnit structureUnit = structureTutorial.Units[unit];

			EducationProgressDto taskProgress = educationProgress
				.Where(dto => dto.Tutorial == tutorial)
				.Where(dto => dto.Unit == unit)
				.First(dto => dto.Task == task);

			bool lowTaskScore = !taskProgress.IsOk();
			int retries = taskProgress.Retries.GetValueOrDefault();
			var tasksByTypeInfo = new Lazy<TaskByTypeInfo>(() => new TaskByTypeInfo(educationProgress));
			bool passedAllWithoutRetries = educationProgress.ForAll(dto => dto.Retries == null && dto.IsOk());

			//закончил последний урок по первой дисциплине
			achievements.SetAchievement(UserAchievement.Voila, () => tutorial == EducationTutorial.PersonalFinance && unit == structureTutorial.Units.Count && task == structureUnit.Tasks.Count)

			//за первое провальное прохождение теста/тру фолс
			.SetAchievement(UserAchievement.GreenArrow, () => taskType == EducationTaskType.TrueFalse && retries == 0 && lowTaskScore)

			//не пройти задание с трех раз (т.е. после 2х сбросов)
			.SetAchievement(UserAchievement.Paradox, () => retries == 2 && lowTaskScore)

			//сдать с 3 раза тест/тру фолс
			.SetAchievement(UserAchievement.Trinity, () => taskType == EducationTaskType.TrueFalse && retries == 2 && !lowTaskScore)

			//прошел первую дисциплину на 100%
			.SetAchievement(UserAchievement.NowIKnow, () => educationProgress.Where(dto => dto.Tutorial == EducationTutorial.PersonalFinance).ForAll(dto => dto.IsMax()))

			//прошел все уроки одного типа
			.SetAchievement(UserAchievement.IveSeenThis, () => tasksByTypeInfo.Value.Data.Any(grouping => grouping.ForAll(arg => arg.HasProgress)))

			//прошел все уроки одного типа на 100%
			.SetAchievement(UserAchievement.Bullseye, () => tasksByTypeInfo.Value.Data.Any(grouping => grouping.ForAll(arg => arg.Value == Progress.MaxProgress)))

			//прошел всё с одного раза не менее, чем с 80% результатом
			.SetAchievement(UserAchievement.Split, () => passedAllWithoutRetries)

			//пройти все unit с одинаковым результатом с первого раза
			.SetAchievement(UserAchievement.Stability, () =>
			{
				if (!passedAllWithoutRetries)
					return false;

				IEnumerable<double> procents = educationProgress
					.GroupBy(dto => new {dto.Tutorial, dto.Unit})
					.Select(dtos => dtos.Average(dto => dto.Value.GetValueOrDefault()))
					.Distinct();

				return procents.Count() == 1;
			})

			//прошел больше одного Unit за день
			.SetAchievement(UserAchievement.Unstoppable, () =>
			{
				int todayFinishedUnits = educationProgress
					.GroupBy(dto => new {dto.Tutorial, dto.Unit}, dto => new {dto.Value, dto.HasProgress, dto.Date})
					.Count(grouping =>
						grouping.Average(arg => arg.Value) >= Progress.OkProgress
							&& grouping.All(arg => arg.HasProgress && arg.Date.HasValue && arg.Date.Value.Date == DateTime.UtcNow.Date));

				return todayFinishedUnits > 1;
			})

			//вернулся в предыдущие дисциплины, чтобы довести их до 100%
			.SetAchievement(UserAchievement.Insister, () =>
			{
				EducationTutorial currentTutorial = GetCurrentTutorial(educationProgress);
				if (currentTutorial == EducationTutorial.PersonalFinance)
					return false;

				EducationProgressDto[] previousTutorialTasks = educationProgress.Where(dto => dto.Tutorial < currentTutorial).ToArray();

				return previousTutorialTasks.ForAll(dto => dto.IsMax()) && previousTutorialTasks.Any(dto => dto.Retries.GetValueOrDefault() > 0);
			});

			//закончил урок быстрее указанного времени
			//.SetAchievement(UserAchievement.DoubleQuick, () => model.Duration.Seconds < SecondsForTaskSettingsModel.GetDefaultSeconds(taskType));
		}

		private static EducationTutorial GetCurrentTutorial(EducationProgressDto[] educationProgress)
		{
			EducationTutorial currentTutorial = educationProgress
				.OrderByDescending(dto => dto.Tutorial)
				.Where(dto => dto.HasProgress).Select(dto => dto.Tutorial)
				.FirstOrDefault();

			if (currentTutorial != EducationTutorial.Economics && educationProgress
				.Where(dto => dto.Tutorial == currentTutorial)
				.All(dto => dto.HasProgress))
				currentTutorial++;

			return currentTutorial;
		}

		public void CheckTotal(StatusInfo statuses, AchievementInfo achievements)
		{
			//получил все обычные ачивки
			achievements.SetAchievement(UserAchievement.TheSeeker, () => AchievementHelper.HasAllAchievementByType(achievements, AchievementType.Standard))

			//получил все редкие ачивки
			.SetAchievement(UserAchievement.RareCollector, () => AchievementHelper.HasAllAchievementByType(achievements, AchievementType.Rare))

			//получил все супер редкие ачивки
			.SetAchievement(UserAchievement.SuperRareCollector, () => AchievementHelper.HasAllAchievementByType(achievements, AchievementType.SuperRare))

			//получил все ультра редкие ачивки
			.SetAchievement(UserAchievement.UltraRareCollector, () => AchievementHelper.HasAllAchievementByType(achievements, AchievementType.UltraRare))

			//получил все уникальные ачивки
			.SetAchievement(UserAchievement.NotSoHard, () => AchievementHelper.HasAllAchievementByType(achievements, AchievementType.Unique))

			//получил уникальную ачивку
			.SetAchievement(UserAchievement.TaDam, () => AchievementHelper.GetAchievementCount(achievements, type => type == AchievementType.Unique) > 0)

			//получил все ачивки (кроме уникальных)
			.SetAchievement(UserAchievement.Curious, () => AchievementHelper.HasAchievement(achievements, type => type != AchievementType.Unique));

			//получил все статусы
			CheckAllStatusesAchievement(statuses, achievements);
		}

		public void CheckAllStatusesAchievement(StatusInfo statuses, AchievementInfo achievements)
		{
			List<StatusDto> statusesItems = statuses.Items;

			if (Enum.GetValues<UserStatus>().Except(statusesItems.Select(dto => dto.Status)).Any())
				return;

			if (LeveledUserStatus.Any(tuple => statusesItems.First(dto => dto.Status == tuple.status).Level != tuple.maxLevel))
				return;

			achievements.SetAchievement(UserAchievement.CheckMe);
		}
	}
}