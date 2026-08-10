using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SteamTrackerApi.Dto;

namespace SteamTrackerApi.Services;

public class SteamService : ISteamService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public SteamService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("SteamClient");
        _apiKey = configuration["SteamApiKey"] 
            ?? configuration["Steam:ApiKey"] 
            ?? throw new InvalidOperationException("SteamApiKey est manquante dans appsettings.json.");
    }

    public async Task<PlayerDto?> GetPlayerSummaryAsync(string steamId)
    {
        var response = await _httpClient.GetAsync($"ISteamUser/GetPlayerSummaries/v2/?key={_apiKey}&steamids={steamId}");
        
        if (!response.IsSuccessStatusCode)
            return null;

        var stream = await response.Content.ReadAsStreamAsync();
        var steamResponse = await JsonSerializer.DeserializeAsync<SteamPlayerApiResponse>(stream);

        var player = steamResponse?.Response?.Players?.FirstOrDefault();
        if (player == null) 
            return null;

        return new PlayerDto
        {
            SteamId = player.SteamId ?? string.Empty,
            PersonalName = player.PersonaName ?? string.Empty,
            NumberGames = 0, 
            TimePlayed = 0   
        };
    }


    private class SteamPlayerApiResponse
    {
        [JsonPropertyName("response")]
        public SteamPlayerResponseContent? Response { get; set; }
    }

    private class SteamPlayerResponseContent
    {
        [JsonPropertyName("players")]
        public List<RawSteamPlayer>? Players { get; set; }
    }
    
    private class RawSteamPlayer
    {
        [JsonPropertyName("steamid")]
        public string? SteamId { get; set; }

        [JsonPropertyName("personaname")]
        public string? PersonaName { get; set; }
    }
}