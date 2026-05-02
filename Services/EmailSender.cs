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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[EmailSender] Unexpected failure sending to {Email} | Subject: {Subject}",
                    email, subject);
            }
        }
    }
}