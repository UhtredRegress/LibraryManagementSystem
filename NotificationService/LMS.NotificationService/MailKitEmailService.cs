using MailKit.Net.Smtp;
using MimeKit;

namespace LMS.NotificationService;

public class MailKitEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public MailKitEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_config["Smtp:SenderName"], _config["Smtp:SenderEmail"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart("plain") { Text = body };

        using var _smtpClient = new SmtpClient();
        await _smtpClient.ConnectAsync(_config["Smtp:Host"],
                                int.Parse(_config["Smtp:Port"]),
                                MailKit.Security.SecureSocketOptions.None);
        await _smtpClient.SendAsync(email);
        await _smtpClient.DisconnectAsync(true);
    }
}