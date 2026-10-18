using System.Net;
using System.Net.Mail;
using GameLogBack.Interfaces;
using GameLogBack.Settings;
using Microsoft.Extensions.Options;
using Resend;

namespace GameLogBack.Services;

public class EmailSenderHelper : IEmailSenderHelper
{
    private readonly IResend _resend;
    private readonly IOptions<ResendSettings> _resendSettings;

    public EmailSenderHelper(IConfiguration configuration, IResend resend, IOptions<ResendSettings> resendSettings)
    {
        _resend = resend;
        _resendSettings = resendSettings;
    }

    public async Task SendEmail(string to, string subject, string message)
    {
        var mailMessage = new EmailMessage
        {
            From = _resendSettings.Value.EmailAddress,
            To = to,
            Subject = subject,
            HtmlBody = message
        };
        await _resend.EmailSendAsync(mailMessage);
    }
}
