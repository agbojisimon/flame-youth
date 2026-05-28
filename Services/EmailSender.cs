using System.Net;
using System.Net.Mail;
using System.Text;
using GlobalFlameMinistry.API.Configuration;
using GlobalFlameMinistry.API.Interfaces.Email;
using Microsoft.Extensions.Options;

namespace GlobalFlameMinistry.API.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        private MailMessage BuildMailMessage(string from, string displayName, string to, string subject, string body)
        {
            var msg = new MailMessage
            {
                From = new MailAddress(from, displayName, Encoding.UTF8),
                Subject = subject,
                Body = body,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Priority = MailPriority.High,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
            };
            msg.To.Add(new MailAddress(to));
            msg.Headers.Add("X-Mailer", "GFM App Mailer");
            return msg;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException("[EmailSender] Cannot send email: recipient address is null or empty.");

            if (string.IsNullOrWhiteSpace(subject))
                throw new InvalidOperationException("[EmailSender] Cannot send email: subject is null or empty.");

            if (string.IsNullOrWhiteSpace(htmlMessage))
                throw new InvalidOperationException("[EmailSender] Cannot send email: body is null or empty.");

            if (string.IsNullOrWhiteSpace(_emailSettings.Server))
                throw new InvalidOperationException("[EmailSender] SMTP server is not configured. Set EmailSettings:Server.");

            if (string.IsNullOrWhiteSpace(_emailSettings.Email))
                throw new InvalidOperationException("[EmailSender] SMTP username (EmailSettings:Email) is not configured.");

            if (string.IsNullOrWhiteSpace(_emailSettings.Password))
                throw new InvalidOperationException("[EmailSender] SMTP password (EmailSettings:Password) is not configured.");

            if (string.IsNullOrWhiteSpace(_emailSettings.SenderEmail))
                throw new InvalidOperationException("[EmailSender] Sender email (EmailSettings:SenderEmail) is not configured.");

            if (string.IsNullOrWhiteSpace(_emailSettings.SenderName))
                throw new InvalidOperationException("[EmailSender] Sender name (EmailSettings:SenderName) is not configured.");

            try
            {
                using var smtpClient = new SmtpClient(_emailSettings.Server)
                {
                    Port = _emailSettings.Port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(
                        _emailSettings.Email,
                        _emailSettings.Password)
                };

                using var msg = BuildMailMessage(
                    _emailSettings.SenderEmail,
                    _emailSettings.SenderName,
                    email,
                    subject,
                    htmlMessage);

                await smtpClient.SendMailAsync(msg);

                _logger.LogInformation(
                    "[EmailSender] Email sent successfully to {Email} | Subject: {Subject}",
                    email, subject);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex,
                    "[EmailSender] SMTP failure sending to {Email} | Subject: {Subject} | StatusCode: {StatusCode}",
                    email, subject, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[EmailSender] Unexpected failure sending to {Email} | Subject: {Subject}",
                    email, subject);
                throw;
            }
        }
    }
}
