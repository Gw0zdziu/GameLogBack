namespace GameLogBack.Dtos.Game;

public class GameByCategoryIdDto
{
    public string GameId { get; set; }
    public string GameName { get; set; }
    public string CategoryId { get; set; }
    public string CategoryName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? YearPlayed { get; set; }
}