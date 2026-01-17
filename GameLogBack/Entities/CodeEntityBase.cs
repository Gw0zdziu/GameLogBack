namespace GameLogBack.Entities;

public abstract class CodeEntityBase
{
    public string CodeId { get; set; }
    public string UserId { get; set; }
    public string Code { get; set; }
    public DateTime ExpiryDate { get; set; }
    public virtual Users User { get; set; }
}