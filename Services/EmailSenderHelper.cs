using System.Net;
using System.Net.Mail;
using GameLogBack.Configurations;
using GameLogBack.Interfaces;
using Microsoft.Extensions.Options;

namespace GameLogBack.Services;

public class EmailSenderHelper : IEmailSenderHelper
{
    private readonly SmtpSettings _smtpSettings;

    public EmailSenderHelper(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public async Task SendEmail(string to, string subject, string message)
    {
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(_smtpSettings.Address);
        mailMessage.To.Add(to);
        mailMessage.Subject = subject;
        mailMessage.Body = message;
        SmtpClient smtpClient = new SmtpClient(_smtpSettings.SmtpServer, _smtpSettings.Port);
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(_smtpSettings.Address, _smtpSettings.Password);
        await smtpClient.SendMailAsync(mailMessage);
    }
}