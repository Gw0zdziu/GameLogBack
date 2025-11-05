namespace GameLogBack.Interfaces;

public interface IEmailSenderHelper
{
    public Task SendEmail(string to, string subject, string message);
}