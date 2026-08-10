namespace SteamTrackerApi.Dto;


public class PlayerDto
{
    public string SteamId { get; set; }
    public string PersonalName { get; set; }
    public int NumberGames { get; set; }
    public float TimePlayed { get; set; }
}
