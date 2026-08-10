using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SteamTrackerApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddHttpClient("SteamClient", client =>
{
    var baseUrl = builder.Configuration["Steam:BaseUrl"] ?? "https://api.steampowered.com/";
    client.BaseAddress = new Uri(baseUrl);
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