using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SteamTrackerApi.Dto;

namespace SteamTrackerApi.Services;

public class AchievementService : IAchievementService
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;

    public AchievementService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        _endpoint = configuration["Steam:Endpoints:GlobalAchievement"] 
                    ?? "ISteamUserStats/GetGlobalAchievementPercentagesForApp/v0002/";
    }
    
    public async Task<List<AchievementDto>> GetAllAchievementsAsync(string gameId, CancellationToken cancellationToken = default)
    {
        var url = $"{_endpoint}?gameid={gameId}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return new List<AchievementDto>();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        
        var options = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        var apiData = await JsonSerializer.DeserializeAsync<AchievementApiResponse>(stream, options, cancellationToken);

        var achievements = apiData?.AchievementPercentages?.Achievements;
        if (achievements == null)
            return new List<AchievementDto>();

        return achievements.Select(a => new AchievementDto
        {
            Id = a.Name,
            Percentage = a.Percent
        }).ToList();
    }

    public async Task<AchievementDto?> GetAchievementAsync(string gameId, string achievementId, CancellationToken cancellationToken = default)
    {
        var allAchievements = await GetAllAchievementsAsync(gameId, cancellationToken);
        return allAchievements.FirstOrDefault(a => string.Equals(a.Id, achievementId, StringComparison.OrdinalIgnoreCase));
    }

   

    private class AchievementApiResponse
    {
        [JsonPropertyName("achievementpercentages")]
        public AchievementPercentagesContent? AchievementPercentages { get; set; }
    }

    private class AchievementPercentagesContent
    {
        [JsonPropertyName("achievements")]
        public List<RawAchievement>? Achievements { get; set; }
    }

    private class RawAchievement
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("percent")]
        public double Percent { get; set; }
    }

    
}