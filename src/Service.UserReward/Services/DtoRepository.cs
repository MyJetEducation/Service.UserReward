using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Constants;
using Service.Core.Client.Extensions;
using Service.Core.Client.Models;
using Service.EducationProgress.Domain.Models;
using Service.ServerKeyValue.Grpc;
using Service.ServerKeyValue.Grpc.Models;
using Service.UserReward.Models;

namespace Service.UserReward.Services
{
	public class DtoRepository : IDtoRepository
	{
		private readonly IServerKeyValueService _serverKeyValueService;
		private readonly ILogger<DtoRepository> _logger;

		public DtoRepository(IServerKeyValueService serverKeyValueService, ILogger<DtoRepository> logger)
		{
			_serverKeyValueService = serverKeyValueService;
			_logger = logger;
		}

		public async ValueTask<(StatusInfo, AchievementInfo)> GetAll(Guid? userId)
		{
			return (new StatusInfo(await GetStatuses(userId)), new AchievementInfo(await GetAchievements(userId)));
		}

		public async ValueTask<List<StatusDto>> GetStatuses(Guid? userId)
		{
			StatusDto[] statusDtos = await GetDataArray<StatusDto>(Program.ReloadedSettings(model => model.KeyUserStatus), userId);

			return statusDtos.ToList();
		}

		public async ValueTask<List<UserAchievement>> GetAchievements(Guid? userId)
		{
			UserAchievement[] achievementDtos = await GetDataArray<UserAchievement>(Program.ReloadedSettings(model => model.KeyUserAchievement), userId);

			return achievementDtos.ToList();
		}

		public async ValueTask<EducationProgressDto[]> GetEducationProgress(Guid? userId)
		{
			return await GetDataArray<EducationProgressDto>(Program.ReloadedSettings(model => model.KeyEducationProgress), userId);
		}

		public async ValueTask<bool> SetStatuses(Guid? userId, StatusInfo statuses)
		{
			CommonGrpcResponse commonGrpcResponse = await SetData(Program.ReloadedSettings(model => model.KeyUserStatus), userId, statuses.Items);

			return commonGrpcResponse.IsSuccess;
		}

		public async ValueTask<bool> SetAchievements(Guid? userId, AchievementInfo achievements)
		{
			CommonGrpcResponse commonGrpcResponse = await SetData(Program.ReloadedSettings(model => model.KeyUserAchievement), userId, achievements.Items);

			return commonGrpcResponse.IsSuccess;
		}

		public async ValueTask<NewAchievementsDto> GetNewAchievements(Guid? userId)
		{
			return await GetDataSingle<NewAchievementsDto>(Program.ReloadedSettings(model => model.KeyUserNewAchievement), userId);
		}

		public async ValueTask<CommonGrpcResponse> SetNewAchievements(Guid? userId, NewAchievementsDto dto)
		{
			Func<string> keyFunc = Program.ReloadedSettings(model => model.KeyUserNewAchievement);

			if (dto.Achievements.IsNullOrEmpty())
				return await _serverKeyValueService.Delete(new ItemsDeleteGrpcRequest
				{
					UserId = userId,
					Keys = new[] {keyFunc.Invoke()}
				});

			return await SetData(keyFunc, userId, dto);
		}

		public async ValueTask<CommonGrpcResponse> ClearTestTasks100Prc(Guid? userId)
		{
			return await SetData(Program.ReloadedSettings(model => model.KeyTestTasks100Prc), userId, new TestTasks100PrcDto());
		}

		public async ValueTask<TestTasks100PrcDto> GetTestTasks100Prc(Guid? userId)
		{
			return await GetDataSingle<TestTasks100PrcDto>(Program.ReloadedSettings(model => model.KeyTestTasks100Prc), userId)
				?? new TestTasks100PrcDto();
		}

		private async ValueTask<TDto[]> GetDataArray<TDto>(Func<string> settingsKeyFunc, Guid? userId)
		{
			string value = (await _serverKeyValueService.GetSingle(new ItemsGetSingleGrpcRequest
			{
				UserId = userId,
				Key = settingsKeyFunc.Invoke()
			}))?.Value;

			return value == null
				? Array.Empty<TDto>()
				: JsonSerializer.Deserialize<TDto[]>(value);
		}

		private async ValueTask<TDto> GetDataSingle<TDto>(Func<string> settingsKeyFunc, Guid? userId) where TDto : class
		{
			string value = (await _serverKeyValueService.GetSingle(new ItemsGetSingleGrpcRequest
			{
				UserId = userId,
				Key = settingsKeyFunc.Invoke()
			}))?.Value;

			return value == null
				? await ValueTask.FromResult<TDto>(null)
				: JsonSerializer.Deserialize<TDto>(value);
		}

		private async ValueTask<CommonGrpcResponse> SetData<TDto>(Func<string> settingsKeyFunc, Guid? userId, TDto dto) where TDto : class
		{
			CommonGrpcResponse response = await _serverKeyValueService.Put(new ItemsPutGrpcRequest
			{
				UserId = userId,
				Items = new[]
				{
					new KeyValueGrpcModel
					{
						Key = settingsKeyFunc.Invoke(),
						Value = JsonSerializer.Serialize(dto)
					}
				}
			});

			if (!response.IsSuccess)
				_logger.LogError("Can't save new data of type {type} for {user}", typeof (TDto).Name, userId);

			return response;
		}
	}
}