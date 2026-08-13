using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SteamTrackerApi.Dto;
using SteamTrackerApi.Services;

namespace SteamTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AchievementsController : ControllerBase
{
    private readonly IAchievementService _achievementService;

    public AchievementsController(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    [HttpGet("{gameId}")]
    [ProducesResponseType(typeof(List<AchievementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AchievementDto>>> GetAllAchievements(
        string gameId, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(gameId))
        {
            return BadRequest("L'identifiant du jeu est obligatoire.");
        }

        var achievements = await _achievementService.GetAllAchievementsAsync(gameId, cancellationToken);

        if (achievements == null || achievements.Count == 0)
        {
            return NotFound($"Aucun succès trouvé pour le jeu '{gameId}'.");
        }

        return Ok(achievements);
    }

    [HttpGet("{gameId}/{achievementId}")]
    [ProducesResponseType(typeof(AchievementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AchievementDto>> GetAchievement(
        string gameId, 
        string achievementId, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(gameId) || string.IsNullOrWhiteSpace(achievementId))
        {
            return BadRequest("L'identifiant du jeu et du succès sont obligatoires.");
        }

        var achievement = await _achievementService.GetAchievementAsync(gameId, achievementId, cancellationToken);

        if (achievement == null)
        {
            return NotFound($"Succès '{achievementId}' non trouvé pour le jeu '{gameId}'.");
        }

        return Ok(achievement);
    }
}