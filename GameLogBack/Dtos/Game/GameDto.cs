namespace GameLogBack.Dtos.Game;

public class GameDto
{
    public string GameId { get; set; }
    public string GameName { get; set; }
    public string CategoryId { get; set; }
    public string CategoryName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}