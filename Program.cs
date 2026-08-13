using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using SteamTrackerApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Enregistrement de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Steam Tracker API",
        Version = "v1",
        Description = "API Web ASP.NET Core pour consulter les profils et succès Steam."
    });
});

// Enregistrement des HttpClients typés
builder.Services.AddHttpClient<ISteamService, SteamService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["Steam:BaseUrl"] ?? "https://api.steampowered.com/";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient<IAchievementService, AchievementService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["Steam:BaseUrl"] ?? "https://api.steampowered.com/";
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Steam Tracker API v1");
        c.RoutePrefix = "swagger"; 
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();