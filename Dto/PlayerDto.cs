namespace SteamTrackerApi.Dto;


public class PlayerDto
{
    public string SteamId { get; set; }
    public string PersonalName { get; set; }
    public string PictureAvatar { get; set; }
    public int NumberGames { get; set; }
    public string TimePlayed { get; set; }
    public string TimePlayed2Week { get; set; }
}
