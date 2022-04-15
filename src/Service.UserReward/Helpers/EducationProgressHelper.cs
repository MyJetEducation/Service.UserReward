using Service.Education.Constants;
using Service.EducationProgress.Grpc.Models;

namespace Service.UserReward.Helpers
{
	public static class EducationProgressHelper
	{
		public static bool IsOk(this EducationProgressTaskDataGrpcModel dto) => dto.Value >= Progress.OkProgress;

		public static bool IsMax(this EducationProgressTaskDataGrpcModel dto) => dto.Value == Progress.MaxProgress;

		public static bool IsMin(this EducationProgressTaskDataGrpcModel dto) => dto.Value == Progress.MinProgress;
	}
}