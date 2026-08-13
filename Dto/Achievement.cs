using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SteamTrackerApi.Dto;

public class AchievementDto
{
    public string Id { get; set; } = string.Empty;
    public double Percentage { get; set; }
}


public class AchievementApiResponse
{
    [JsonPropertyName("achievementpercentages")]
    public AchievementPercentagesContainer? AchievementPercentages { get; set; }
}

public class AchievementPercentagesContainer
{
    [JsonPropertyName("achievements")]
    public List<SteamAchievementItem>? Achievements { get; set; }
}

public class SteamAchievementItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("percent")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public double Percent { get; set; }
}

