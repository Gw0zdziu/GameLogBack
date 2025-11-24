namespace GameLogBack.Entities;

public class RefreshTokenInfo
{
    public string RefreshTokenId { get; set; }
    public string UserId { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiryDate { get; set; }
    public virtual Users User { get; set; }
}