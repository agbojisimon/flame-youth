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

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        private MailMessage MailMessageServer(string from, string displayName, string to, string subject, string body)
        {
            var msg = new MailMessage()
            {
                From = new MailAddress(from, displayName, Encoding.UTF8)
            };
            msg.To.Add(new MailAddress(to));
            msg.Subject = subject;
            msg.Body = body;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = true;
            msg.Headers.Add("Mail", "App Mail");
            msg.Priority = MailPriority.High;
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

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
                    Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password)
                };

                using var msg = MailMessageServer(_emailSettings.Email, _emailSettings.SenderName, email, subject, htmlMessage);

                await smtpClient.SendMailAsync(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email send failed: {ex.Message}");
                throw;
            }
        }
    }
}