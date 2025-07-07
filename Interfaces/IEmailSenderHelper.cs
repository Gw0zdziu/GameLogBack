namespace GameLogBack.Interfaces;

public interface IEmailSenderHelper
{
    public void SendEmail(string to, string subject, string message);
}