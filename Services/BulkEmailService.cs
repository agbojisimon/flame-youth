using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using GlobalFlameMinistry.API.Configuration;
using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.BulkEmail;
using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces.BulkEmail;
using GlobalFlameMinistry.API.Mappers;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GlobalFlameMinistry.API.Services
{
    public class BulkEmailService : IBulkEmailService
    {
        private const int EmailBatchSize = 50;

        // Internal record — email + name pair for personalization
        private record EmailRecipient(string Email, string FirstName);

        private readonly IBulkEmailRepository _repo;
        private readonly AppDbContext _context;
        private readonly BrevoSettings _brevo;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BulkEmailService> _logger;

        public BulkEmailService(
            IBulkEmailRepository repo,
            AppDbContext context,
            IOptions<BrevoSettings> brevoOptions,
            IHttpClientFactory httpClientFactory,
            ILogger<BulkEmailService> logger)
        {
            _repo = repo;
            _context = context;
            _brevo = brevoOptions.Value;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // ── SEND NOW ────────────────────────────────────────────────────────
        public async Task<BulkEmailResponseDto> SendNowAsync(
            SendBulkEmailDto dto, string adminUserId, string adminName)
        {
            var recipients = await ResolveRecipientsAsync(dto);
            var htmlBodyTemplate = BuildHtmlTemplate(dto.Subject, dto.HtmlBody, dto.ImageUrl);

            var message = dto.ToModel(
                htmlBodyTemplate, recipients.Count, "Sending", adminUserId, adminName);

            var created = await _repo.CreateAsync(message);
            await DispatchEmailsAsync(created, recipients);

            // Re-fetch to return the actual final status — not the stale "Sending" state
            var updated = await _repo.GetByIdAsync(created.Id);
            return updated!.ToBulkEmailResponseDto();
        }

        // ── SCHEDULE ────────────────────────────────────────────────────────
        public async Task<BulkEmailResponseDto> ScheduleAsync(
            SendBulkEmailDto dto, string adminUserId, string adminName)
        {
            var recipients = await ResolveRecipientsAsync(dto);
            var htmlBodyTemplate = BuildHtmlTemplate(dto.Subject, dto.HtmlBody, dto.ImageUrl);

            var message = dto.ToModel(
                htmlBodyTemplate, recipients.Count, "Scheduled", adminUserId, adminName);

            var created = await _repo.CreateAsync(message);
            return created.ToBulkEmailResponseDto();
        }

        // ── GET HISTORY ─────────────────────────────────────────────────────
        public async Task<PagedResult<BulkEmailResponseDto>> GetHistoryAsync(
            BulkEmailQueryObject query)
        {
            var messages = await _repo.GetAllAsync(query);
            var totalCount = await _repo.GetCountAsync(query);

            return new PagedResult<BulkEmailResponseDto>
            {
                Items = messages.ToDtoList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        // ── GET STATS ───────────────────────────────────────────────────────
        public async Task<BulkEmailStatsDto> GetStatsAsync()
        {
            return await _repo.GetStatsAsync();
        }

        // ── CANCEL SCHEDULED ────────────────────────────────────────────────
        public async Task<bool> CancelScheduledAsync(int id)
        {
            return await _repo.CancelAsync(id);
        }

        // ── PROCESS SCHEDULED (called by background service) ────────────────
        public async Task ProcessScheduledEmailsAsync()
        {
            var dueMessages = await _repo.GetDueScheduledAsync();

            foreach (var message in dueMessages)
            {
                var recipients = await ResolveRecipientsFromMessageAsync(message);
                message.Status = "Sending";
                await _repo.UpdateAsync(message);
                await DispatchEmailsAsync(message, recipients);
            }
        }

        // ── PRIVATE: DISPATCH TO BREVO VIA HTTP ─────────────────────────────
        private async Task DispatchEmailsAsync(
            BulkEmailMessage message, List<EmailRecipient> recipients)
        {
            var httpClient = _httpClientFactory.CreateClient("BrevoClient");
            httpClient.DefaultRequestHeaders.Remove("api-key");
            httpClient.DefaultRequestHeaders.Add("api-key", _brevo.ApiKey);

            try
            {
                int successCount = 0;
                int failedCount = 0;

                // Batch into groups for rate limit compliance
                var batches = recipients
                    .Select((r, index) => new { r, index })
                    .GroupBy(x => x.index / EmailBatchSize)
                    .Select(g => g.Select(x => x.r).ToList())
                    .ToList();

                foreach (var batch in batches)
                {
                    foreach (var recipient in batch)
                    {
                        try
                        {
                            // Personalize the body for this specific recipient
                            var personalizedBody = message.HtmlBody
                                .Replace("{{firstName}}", recipient.FirstName);

                            var payload = new
                            {
                                sender = new
                                {
                                    name = _brevo.SenderName,
                                    email = _brevo.SenderEmail
                                },
                                to = new[]
                                {
                                    new { email = recipient.Email }  // one recipient only — no exposure
                                },
                                subject = message.Subject,
                                htmlContent = personalizedBody
                            };

                            var json = JsonSerializer.Serialize(payload);
                            var content = new StringContent(
                                json, Encoding.UTF8, "application/json");

                            var response = await httpClient.PostAsync("smtp/email", content);
                            var responseBody = await response.Content.ReadAsStringAsync();

                            if (response.IsSuccessStatusCode)
                            {
                                successCount++;
                                _logger.LogInformation(
                                    "Email sent to {Email} for BulkEmail {Id}",
                                    recipient.Email, message.Id);
                            }
                            else
                            {
                                failedCount++;
                                _logger.LogError(
                                    "Brevo rejected email to {Email} for BulkEmail {Id}. " +
                                    "Status: {Status}. Response: {Body}",
                                    recipient.Email, message.Id,
                                    response.StatusCode, responseBody);
                            }

                            // Small delay between individual sends
                            await Task.Delay(100);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "Exception sending to {Email} for BulkEmail {Id}",
                                recipient.Email, message.Id);
                            failedCount++;
                        }
                    }

                    // Polite delay between batches
                    await Task.Delay(500);
                }

                message.Status = failedCount == recipients.Count ? "Failed" : "Sent";
                message.SuccessCount = successCount;
                message.FailedCount = failedCount;
                message.SentAt = DateTime.UtcNow;

                await _repo.UpdateAsync(message);

                _logger.LogInformation(
                    "BulkEmail {Id} complete. Sent: {Success}, Failed: {Failed}",
                    message.Id, successCount, failedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Dispatch completely failed for BulkEmail {Id}", message.Id);
                message.Status = "Failed";
                message.ErrorMessage = ex.Message;
                await _repo.UpdateAsync(message);
            }
        }

        // ── PRIVATE: RESOLVE RECIPIENTS ─────────────────────────────────────
        private async Task<List<EmailRecipient>> ResolveRecipientsAsync(SendBulkEmailDto dto)
        {
            if (dto.TargetGroup == "Custom")
            {
                if (string.IsNullOrWhiteSpace(dto.CustomEmails))
                    return new List<EmailRecipient>();

                // Custom emails have no names — fall back to "Member"
                return dto.CustomEmails
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Distinct()
                    .Select(e => new EmailRecipient(e, "Beloved"))
                    .ToList();
            }

            var query = _context.Users.Where(u => u.EmailConfirmed);

            if (dto.TargetGroup == "Ministry")
                query = query.Where(u => u.Module == "Ministry");
            else if (dto.TargetGroup == "Youth")
                query = query.Where(u => u.Module == "Youth");

            return await query
                .Where(u => !string.IsNullOrWhiteSpace(u.Email))
                .Select(u => new EmailRecipient(u.Email!, u.FirstName))
                .ToListAsync();
        }

        private async Task<List<EmailRecipient>> ResolveRecipientsFromMessageAsync(
            BulkEmailMessage message)
        {
            if (message.TargetGroup == "Custom" &&
                !string.IsNullOrWhiteSpace(message.CustomEmailsJson))
            {
                return message.CustomEmailsJson
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .Select(e => new EmailRecipient(e, "Member"))
                    .ToList();
            }

            var query = _context.Users.Where(u => u.EmailConfirmed);

            if (message.TargetGroup == "Ministry")
                query = query.Where(u => u.Module == "Ministry");
            else if (message.TargetGroup == "Youth")
                query = query.Where(u => u.Module == "Youth");

            return await query
                .Where(u => !string.IsNullOrWhiteSpace(u.Email))
                .Select(u => new EmailRecipient(u.Email!, u.FirstName))
                .ToListAsync();
        }

        private static string BuildHtmlTemplate(string subject, string body, string? imageUrl)
        {
            var encodedSubject = HtmlEncoder.Default.Encode(subject);

            var encodedBody = HtmlEncoder.Default.Encode(body)
                .Replace("&#xA;", "<br/>")
                .Replace("&#xD;&#xA;", "<br/>");

            // Optional image block — only rendered if imageUrl is provided
            var imageBlock = !string.IsNullOrWhiteSpace(imageUrl)
                ? $@"<tr>
               <td style='padding:0;'>
                 <img src='{imageUrl}' alt='Email Image'
                   style='width:100%;max-width:600px;display:block;
                          border-radius:0;object-fit:cover;max-height:300px;'/>
               </td>
             </tr>"
                : string.Empty;

            // Logo hosted on your domain or a CDN
            // Using an absolute URL so email clients can load it
            const string logoUrl = "https://res.cloudinary.com/dveeb0yop/image/upload/v1777117674/flames_hmyf0q.jpg";

            return "<!DOCTYPE html><html><head>" +
                   "<meta charset='UTF-8'/>" +
                   "<meta name='viewport' content='width=device-width, initial-scale=1.0'/>" +
                   "<title>" + encodedSubject + "</title></head>" +
                   "<body style='margin:0;padding:0;background:#f4f4f7;font-family:Arial,sans-serif;'>" +
                   "<table width='100%' cellpadding='0' cellspacing='0' style='background:#f4f4f7;padding:40px 0;'><tr><td align='center'>" +
                   "<table width='600' cellpadding='0' cellspacing='0' style='background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,0.08);'>" +

                   // ── HEADER with logo ──
                   "<tr><td style='background:linear-gradient(135deg,#a21caf,#7c3aed);padding:28px 40px;text-align:center;'>" +
                   "<img src='" + logoUrl + "' alt='Global Flame' " +
                   "style='width:48px;height:48px;object-fit:cover;" +
                   "display:block;margin:0 auto 12px;'/>" +
                   "<h1 style='color:#ffffff;margin:0;font-size:20px;font-weight:bold;letter-spacing:1px;'>GLOBAL FLAME</h1>" +
                   "<p style='color:rgba(255,255,255,0.8);margin:4px 0 0;font-size:9px;'>Raising a people of power who will manifest the kingdom</p>" +
                   "</td></tr>" +

                   // ── SUBJECT BAR ──
                   "<tr><td style='background:#faf5ff;padding:20px 40px;border-bottom:2px solid #e9d5ff;'>" +
                   "<h2 style='color:#7c3aed;margin:0;font-size:18px;'>" + encodedSubject + "</h2>" +
                   "</td></tr>" +

                   // ── OPTIONAL IMAGE ──
                   imageBlock +

                   // ── BODY ──
                   "<tr><td style='padding:32px 40px;color:#374151;font-size:15px;line-height:1.7;'>" +
                   "<p style='margin:0 0 16px;'>Dear {{firstName}},</p>" +
                   encodedBody +
                   "</td></tr>" +

                   // ── FOOTER ──
                   "<tr><td style='background:#f9fafb;padding:24px 40px;text-align:center;border-top:1px solid #e5e7eb;'>" +
                   "<p style='color:#9ca3af;font-size:12px;margin:0 0 8px;'>You are receiving this email because you are a registered member of Global Flame Ministry.</p>" +
                   "<p style='color:#9ca3af;font-size:12px;margin:0;'>© " + DateTime.UtcNow.Year + " Global Flame Ministry. All rights reserved.</p>" +
                   "</td></tr>" +

                   "</table></td></tr></table></body></html>";
        }
    }
}