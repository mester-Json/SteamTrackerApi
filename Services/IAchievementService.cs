using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SteamTrackerApi.Dto;

namespace SteamTrackerApi.Services;

public interface IAchievementService
{
    Task<AchievementDto?> GetAchievementAsync(string gameId, string achievementId, CancellationToken cancellationToken = default);
    
    Task<List<AchievementDto>> GetAllAchievementsAsync(string gameId, CancellationToken cancellationToken = default);
}