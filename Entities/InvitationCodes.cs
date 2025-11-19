namespace GameLogBack.Entities;

public class InvitationCodes
{
    public string InvitationCodeId { get; set; }
    public string InvitationCode { get; set; }
    public bool IsUsed { get; set; }
}