using System.Collections.Generic;
using System.Linq;
using Service.Education.Helpers;
using Service.Education.Structure;
using Service.EducationProgress.Grpc.Models;

namespace Service.UserReward.Services
{
	public class TaskByTypeInfo
	{
		public TaskByTypeInfo(IEnumerable<EducationProgressTaskDataGrpcModel> educationProgress)
		{
			Data = educationProgress.Select(dto => new TaskByTypeDto
			{
				Value = dto.Value,
				HasProgress = dto.HasProgress(),
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