using Service.Core.Client.Constants;
using Service.EducationProgress.Domain.Models;

namespace Service.UserReward.Helpers
{
	public static class EducationProgressHelper
	{
		public static bool IsOk(this EducationProgressDto dto) => dto.Value >= AnswerProgress.OkAnswerProgress;

		public static bool IsMax(this EducationProgressDto dto) => dto.Value == AnswerProgress.MaxAnswerProgress;

		public static bool IsMin(this EducationProgressDto dto) => dto.Value == AnswerProgress.MinAnswerProgress;
	}
}