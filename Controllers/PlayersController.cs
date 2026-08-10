using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SteamTrackerApi.Dto;
using SteamTrackerApi.Services;

namespace SteamTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly ISteamService _steamService;

    public PlayersController(ISteamService steamService)
    {
        _steamService = steamService;
    }

    [HttpGet("{steamId}")]
    public async Task<ActionResult<PlayerDto>> GetPlayer(string steamId)
    {
        var player = await _steamService.GetPlayerSummaryAsync(steamId);

        if (player == null)
        {
            return NotFound(new { message = $"Joueur avec le SteamID '{steamId}' introuvable ou profil privé." });
        }

        return Ok(player);
    }
}