namespace GameLogBack.Entities;

public class ConfirmCodeUsers
{
    public string ConfirmCodeId { get; set; }
    public string UserId { get; set; }
    public string ConfirmCode { get; set; }
    public DateTime ExpiryDate { get; set; }
    public Users User { get; set; }
}