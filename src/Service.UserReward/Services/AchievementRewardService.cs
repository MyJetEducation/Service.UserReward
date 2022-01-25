using System;
using System.Collections.Generic;
using System.Linq;
using Service.Core.Domain.Models.Constants;
using Service.Core.Domain.Models.Education;
using Service.EducationProgress.Domain.Models;
using Service.EducationProgress.Grpc.ServiceBusModels;
using Service.UserReward.Domain.Models;
using Service.UserReward.Helpers;
using Service.UserReward.Settings;

namespace Service.UserReward.Services
{
	public class AchievementRewardService : IAchievementRewardService
	{
		public bool CheckByProgress(SetProgressInfoServiceBusModel model, EducationProgressDto[] educationProgress, List<UserAchievement> achievements)
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

			//закончил последний урок по первой дисциплине
			bool changed = achievements.SetAchievement(UserAchievement.Voila, () => tutorial == EducationTutorial.PersonalFinance && unit == structureTutorial.Units.Count && task == structureUnit.Tasks.Count);

			//за первое провальное прохождение теста/тру фолс
			changed = changed || achievements.SetAchievement(UserAchievement.GreenArrow, () => taskType == EducationTaskType.TrueFalse && retries == 0 && lowTaskScore);

			//не пройти задание с трех раз (т.е. после 2х сбросов)
			changed = changed || achievements.SetAchievement(UserAchievement.Paradox, () => retries == 2 && lowTaskScore);

			//сдать с 3 раза тест/тру фолс
			changed = changed || achievements.SetAchievement(UserAchievement.Trinity, () => taskType == EducationTaskType.TrueFalse && retries == 2 && !lowTaskScore);

			//прошел первую дисциплину на 100%
			changed = changed || achievements.SetAchievement(UserAchievement.NowIKnow, () => educationProgress.Where(dto => dto.Tutorial == EducationTutorial.PersonalFinance).All(dto => dto.IsMax()));

			if (!achievements.Contains(UserAchievement.IveSeenThis) || !achievements.Contains(UserAchievement.Bullseye))
			{
				var tasksByType = educationProgress.Select(dto => new
				{
					dto.Value,
					dto.HasProgress,
					EducationHelper.GetTask(dto.Tutorial, dto.Unit, dto.Task).TaskType
				}).GroupBy(arg => arg.TaskType);

				//прошел все уроки одного типа
				changed = changed || achievements.SetAchievement(UserAchievement.IveSeenThis, () => tasksByType.Any(grouping => grouping.All(arg => arg.HasProgress)));

				//прошел все уроки одного типа на 100%
				changed = changed || achievements.SetAchievement(UserAchievement.Bullseye, () => tasksByType.Any(grouping => grouping.All(arg => arg.Value == AnswerProgress.MaxAnswerProgress)));
			}

			//прошел всё с одного раза не менее, чем с 80% результатом
			bool allWithoutRetries = educationProgress.All(dto => dto.Retries == null && dto.IsOk());
			changed = changed || achievements.SetAchievement(UserAchievement.Split, () => allWithoutRetries);

			//пройти все unit с одинаковым результатом с первого раза
			if (!achievements.Contains(UserAchievement.Stability))
			{
				IEnumerable<double> procents = educationProgress
					.GroupBy(dto => new {dto.Tutorial, dto.Unit})
					.Select(dtos => dtos.Average(dto => dto.Value.GetValueOrDefault()))
					.Distinct();

				changed = changed || achievements.SetAchievement(UserAchievement.Stability, () => allWithoutRetries && procents.Count() == 1);
			}

			//прошел больше одного Unit за день
			if (!achievements.Contains(UserAchievement.Unstoppable))
			{
				int todayFinishedUnits = educationProgress
					.GroupBy(dto => new {dto.Tutorial, dto.Unit}, dto => new {dto.Value, dto.HasProgress, dto.Date})
					.Count(grouping =>
						grouping.Average(arg => arg.Value) >= AnswerProgress.OkAnswerProgress
							&& grouping.All(arg => arg.HasProgress && arg.Date.HasValue && arg.Date.Value.Date == DateTime.UtcNow.Date));

				changed = changed || achievements.SetAchievement(UserAchievement.Unstoppable, () => todayFinishedUnits > 1);
			}

			//вернулся в предыдущие дисциплины, чтобы довести их до 100%
			if (!achievements.Contains(UserAchievement.Insister))
			{
				EducationTutorial currentTutorial = GetCurrentTutorial(educationProgress);
				if (currentTutorial != EducationTutorial.PersonalFinance)
				{
					EducationProgressDto[] previousTutorialTasks = educationProgress.Where(dto => dto.Tutorial < currentTutorial).ToArray();
					changed = changed || achievements.SetAchievement(UserAchievement.Insister, () => previousTutorialTasks.All(dto => dto.IsMax()) && previousTutorialTasks.Any(dto => dto.Retries.GetValueOrDefault() > 0));
				}
			}

			//закончил урок быстрее указанного времени
			changed = changed || achievements.SetAchievement(UserAchievement.DoubleQuick, () => model.Duration.TotalSeconds > SecondsForTaskSettingsModel.GetDefaultSeconds(taskType));

			return changed;
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

		public bool CheckTotal(IEnumerable<StatusDto> statuses, List<UserAchievement> achievements)
		{
			//получил все обычные ачивки
			bool changed = achievements.SetAchievement(UserAchievement.TheSeeker, () => AchievementHelper.HasAllAchievementByType(achievements, AchievementType.Standard))

			//получил все редкие ачивки
			|| achievements.SetAchievement(UserAchievement.RareCollector, () => AchievementHelper.HasAllAchievementByType(achievements, AchievementType.Rare))

			//получил все супер редкие ачивки
			|| achievements.SetAchievement(UserAchievement.SuperRareCollector, () => AchievementHelper.HasAllAchievementByType(achievements, AchievementType.SuperRare))

			//получил все ультра редкие ачивки
			|| achievements.SetAchievement(UserAchievement.UltraRareCollector, () => AchievementHelper.HasAllAchievementByType(achievements, AchievementType.UltraRare))

			//получил все уникальные ачивки
			|| achievements.SetAchievement(UserAchievement.NotSoHard, () => AchievementHelper.HasAllAchievementByType(achievements, AchievementType.Unique))

			//получил уникальную ачивку
			|| achievements.SetAchievement(UserAchievement.TaDam, () => AchievementHelper.GetAchievementCount(achievements, type => type == AchievementType.Unique) > 0)

			//получил все ачивки (кроме уникальных)
			|| achievements.SetAchievement(UserAchievement.Curious, () => AchievementHelper.HasAchievement(achievements, type => type != AchievementType.Unique))

			//получил все статусы
			|| CheckAllStatusesAchievement(statuses, achievements);

			return changed;
		}

		public bool CheckAllStatusesAchievement(IEnumerable<StatusDto> statuses, List<UserAchievement> achievements) => 
			achievements.SetAchievement(UserAchievement.CheckMe, () => statuses.Select(dto => dto.Status).Intersect(Enum.GetValues<UserStatus>()).Any() == false);
	}
}