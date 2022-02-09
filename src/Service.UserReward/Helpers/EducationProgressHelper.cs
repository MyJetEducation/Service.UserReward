using Service.Education.Constants;
using Service.EducationProgress.Domain.Models;

namespace Service.UserReward.Helpers
{
	public static class EducationProgressHelper
	{
		public static bool IsOk(this EducationProgressDto dto) => dto.Value >= Progress.OkProgress;

		public static bool IsMax(this EducationProgressDto dto) => dto.Value == Progress.MaxProgress;

		public static bool IsMin(this EducationProgressDto dto) => dto.Value == Progress.MinProgress;
	}
}