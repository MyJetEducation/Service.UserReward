using System.Collections.Generic;
using System.Linq;
using Service.Core.Domain.Models.Education;
using Service.EducationProgress.Domain.Models;

namespace Service.UserReward.Services
{
	public class TaskByTypeInfo
	{
		public TaskByTypeInfo(IEnumerable<EducationProgressDto> educationProgress)
		{
			Data = educationProgress.Select(dto => new TaskByTypeDto
			{
				Value = dto.Value,
				HasProgress = dto.HasProgress,
				TaskType = EducationHelper.GetTask(dto.Tutorial, dto.Unit, dto.Task).TaskType
			}).GroupBy(arg => arg.TaskType).ToArray();
		}

		public class TaskByTypeDto
		{
			public int? Value { get; set; }
			public bool HasProgress { get; set; }
			public EducationTaskType TaskType { get; set; }
		}

		public IGrouping<EducationTaskType, TaskByTypeDto>[] Data { get; }
	}
}