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
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;
    private readonly string _playerSummaryEndpoint;
    private readonly string _ownedGamesEndpoint;

    public SteamService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        _apiKey = configuration["SteamApiKey"] 
                   ?? configuration["Steam:ApiKey"] 
                   ?? throw new InvalidOperationException("SteamApiKey est manquante dans appsettings.json.");

        _playerSummaryEndpoint = configuration["Steam:Endpoints:PlayerSummary"] ?? "ISteamUser/GetPlayerSummaries/v2/";
        _ownedGamesEndpoint = configuration["Steam:Endpoints:OwnedGames"] ?? "IPlayerService/GetOwnedGames/v1/";
    }

    public async Task<PlayerDto?> GetPlayerSummaryAsync(string steamId)
    {
        var summaryUrl = $"{_playerSummaryEndpoint}?key={_apiKey}&steamids={steamId}";
        var summaryResponse = await _httpClient.GetAsync(summaryUrl);
        
        if (!summaryResponse.IsSuccessStatusCode)
            return null;

        await using var summaryStream = await summaryResponse.Content.ReadAsStreamAsync();
        var steamSummary = await JsonSerializer.DeserializeAsync<SteamPlayerApiResponse>(summaryStream);

        var player = steamSummary?.Response?.Players?.FirstOrDefault();
        if (player == null) 
            return null;

        var gamesUrl = $"{_ownedGamesEndpoint}?key={_apiKey}&steamid={steamId}&include_appinfo=false";
        var gamesResponse = await _httpClient.GetAsync(gamesUrl);
        
        int gameCount = 0;
        int totalPlaytimeMinutes = 0;
        int totalPlaytimeMinutes2Week = 0;

        if (gamesResponse.IsSuccessStatusCode)
        {
            await using var gamesStream = await gamesResponse.Content.ReadAsStreamAsync();
            var steamGames = await JsonSerializer.DeserializeAsync<SteamOwnedGamesApiResponse>(gamesStream);

            gameCount = steamGames?.Response?.GameCount ?? 0;
            totalPlaytimeMinutes = steamGames?.Response?.Games?.Sum(g => g.PlaytimeForever) ?? 0;
            totalPlaytimeMinutes2Week = steamGames?.Response?.Games?.Sum(g => g.Playtime2Week) ?? 0;
        }

        TimeSpan timeSpan = TimeSpan.FromMinutes(totalPlaytimeMinutes);
        TimeSpan timeSpan2 = TimeSpan.FromMinutes(totalPlaytimeMinutes2Week);
        
        string formattedTimePlayed = $"{timeSpan.Days}j {timeSpan.Hours}h";
        string formattedTimePlayed2Week = $"{timeSpan2.Days}j {timeSpan2.Hours}h";

        return new PlayerDto
        {
            SteamId = player.SteamId ?? string.Empty,
            PersonalName = player.PersonaName ?? string.Empty,
            PictureAvatar = player.PictureAvatar ?? string.Empty,
            NumberGames = gameCount,
            TimePlayed = formattedTimePlayed,
            TimePlayed2Week = formattedTimePlayed2Week
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
        
        [JsonPropertyName("avatarfull")]
        public string? PictureAvatar { get; set; }
    }

    private class SteamOwnedGamesApiResponse
    {
        [JsonPropertyName("response")]
        public SteamOwnedGamesResponseContent? Response { get; set; }
    }

    private class SteamOwnedGamesResponseContent
    {
        [JsonPropertyName("game_count")]
        public int GameCount { get; set; }

        [JsonPropertyName("games")]
        public List<RawSteamGame>? Games { get; set; }
    }

    private class RawSteamGame
    {
        [JsonPropertyName("playtime_forever")]
        public int PlaytimeForever { get; set; }
        
        [JsonPropertyName("playtime_2weeks")]
        public int Playtime2Week { get; set; }
    }
}