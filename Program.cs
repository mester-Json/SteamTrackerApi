using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHttpClient("SteamClient", client =>
{
    var baseUrl = builder.Configuration["Steam:BaseUrl"] ?? "https://api.steampowered.com/";
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapGet("/api/steam/player/{steamId}", async (
    string steamId, 
    IHttpClientFactory httpClientFactory, 
    IConfiguration config) =>
{
    var apiKey = config["Steam:ApiKey"];

    if (string.IsNullOrEmpty(apiKey) || apiKey == "TA_CLE_API_STEAM_ICI")
    {
        return Results.BadRequest(new { error = "Pense à mettre ta clé API Steam dans appsettings.json !" });
    }

    var client = httpClientFactory.CreateClient("SteamClient");
    var response = await client.GetAsync($"ISteamUser/GetPlayerSummaries/v2/?key={apiKey}&steamids={steamId}");

    if (!response.IsSuccessStatusCode)
    {
        return Results.Problem("Erreur de réponse de l'API Steam.");
    }

    var content = await response.Content.ReadAsStringAsync();
    return Results.Content(content, "application/json");
});

app.MapGet("/api/steam/player/{steamId}/achievements/{appId}", async (
    string steamId, 
    uint appId, 
    IHttpClientFactory httpClientFactory, 
    IConfiguration config) =>
{
    var apiKey = config["Steam:ApiKey"];

    if (string.IsNullOrEmpty(apiKey) || apiKey == "TA_CLE_API_STEAM_ICI")
    {
        return Results.BadRequest(new { error = "Pense à mettre ta clé API Steam dans appsettings.json !" });
    }

    var client = httpClientFactory.CreateClient("SteamClient");
    var url = $"ISteamUserStats/GetPlayerAchievements/v1/?key={apiKey}&steamid={steamId}&appid={appId}";
    var response = await client.GetAsync(url);

    if (!response.IsSuccessStatusCode)
    {
        return Results.Problem($"Impossible de récupérer les succès (Code: {response.StatusCode}). Le profil est peut-être privé.");
    }

    var content = await response.Content.ReadAsStringAsync();
    return Results.Content(content, "application/json");
});

app.Run();