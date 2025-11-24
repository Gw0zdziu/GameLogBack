namespace GameLogBack.Entities;

public class Games
{
    public string GameId { get; set; }
    public string GameName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public string CategoryId { get; set; }
    public string UserId { get; set; }
    public virtual Categories Category { get; set; }
    public virtual Users User { get; set; }
}