using System.Net;
using System.Net.Mail;
using GameLogBack.Interfaces;
using Microsoft.Extensions.Options;
using Resend;

namespace GameLogBack.Services;

public class EmailSenderHelper : IEmailSenderHelper
{
    private readonly bool _isSendEmail;
    private readonly IResend _resend;
    private readonly string _emailAddress;

    public EmailSenderHelper(IConfiguration configuration, IResend resend)
    {
        _resend = resend;
        _isSendEmail = configuration.GetValue<bool>("SendEmails");
        _emailAddress = configuration.GetValue<string>("EmailAddress");
    }

    public async Task SendEmail(string to, string subject, string message)
    {
        
        if (_isSendEmail)
        {
            var mailMessage = new EmailMessage
            {
                From = _emailAddress,
                To = to,
                Subject = subject,
                HtmlBody = message
            };
            await _resend.EmailSendAsync(mailMessage);
        }
    }
}
