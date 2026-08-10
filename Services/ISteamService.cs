using System.Threading.Tasks;
using SteamTrackerApi.Dto;

namespace SteamTrackerApi.Services;

public interface ISteamService
{
    Task<PlayerDto?> GetPlayerSummaryAsync(string steamId);
}