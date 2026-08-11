using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SteamTrackerApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient("SteamClient", (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
    
    var baseUrl = configuration["Steam:BaseUrl"] ?? "https://api.steampowered.com/";
    client.BaseAddress = new Uri(baseUrl);

    var apiKey = configuration["SteamApiKey"] ?? configuration["Steam:ApiKey"];
    if (string.IsNullOrWhiteSpace(apiKey))
    {
        throw new InvalidOperationException("La clé API Steam ('SteamApiKey' ou 'Steam:ApiKey') est manquante dans appsettings.json.");
    }
});

builder.Services.AddScoped<ISteamService, SteamService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();