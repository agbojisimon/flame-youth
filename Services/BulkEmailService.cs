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

        // ══════════════════════════════════════════════════════════════════
        // DIAGNOSTIC HELPERS  (REMOVE AFTER DEBUGGING)
        // ══════════════════════════════════════════════════════════════════
        private void LogTrace(string step, object? extra = null)
        {
            var extraJson = extra is not null
                ? JsonSerializer.Serialize(extra)
                : "";
            _logger.LogWarning("[DIAG] BULK_EMAIL_TRACE | {Step} | {Extra}",
                step, extraJson);
        }

        private void LogTraceException(string step, Exception ex)
        {
            _logger.LogWarning(ex,
                "[DIAG] BULK_EMAIL_TRACE | {Step} | EXCEPTION | {Message} | {StackTrace}",
                step, ex.Message, ex.StackTrace);
        }
        // ══════════════════════════════════════════════════════════════════

        // ── SEND NOW ────────────────────────────────────────────────────────
        public async Task<BulkEmailResponseDto> SendNowAsync(
            SendBulkEmailDto dto, string adminUserId, string adminName)
        {
            LogTrace("SendNowAsync ENTERED", new { subject = dto.Subject, targetGroup = dto.TargetGroup, customEmails = dto.CustomEmails, scheduledAt = dto.ScheduledAt, adminUserId, adminName });

            if (string.IsNullOrWhiteSpace(dto.Subject))
            {
                LogTrace("SendNowAsync FAILED: Subject is empty");
                throw new InvalidOperationException("[BulkEmail] Subject is required.");
            }
            if (string.IsNullOrWhiteSpace(dto.HtmlBody))
            {
                LogTrace("SendNowAsync FAILED: HtmlBody is empty");
                throw new InvalidOperationException("[BulkEmail] HTML body is required.");
            }

            LogTrace("SendNowAsync calling ResolveRecipientsAsync");
            var recipients = await ResolveRecipientsAsync(dto);
            LogTrace("SendNowAsync ResolveRecipientsAsync returned", new { recipientCount = recipients.Count });

            if (recipients.Count == 0)
            {
                _logger.LogWarning("[BulkEmail] No recipients resolved for SendNow. Subject: {Subject}", dto.Subject);
                LogTrace("SendNowAsync EARLY RETURN: 0 recipients -> hardcoded Status=Sent. NO HTTP REQUEST TO BREVO WILL BE MADE.");
                var empty = dto.ToModel(BuildHtmlTemplate(dto.Subject, dto.HtmlBody, dto.ImageUrl), 0, "Sent", adminUserId, adminName);
                empty.SuccessCount = 0;
                empty.FailedCount = 0;
                empty.Status = "Sent";
                empty.SentAt = DateTime.UtcNow;
                var saved = await _repo.CreateAsync(empty);
                LogTrace("SendNowAsync RETURNING FALSE SUCCESS: DB record saved with Status=Sent but 0 HTTP requests made to Brevo", new { savedId = saved.Id });
                return saved.ToBulkEmailResponseDto();
            }

            var htmlBodyTemplate = BuildHtmlTemplate(dto.Subject, dto.HtmlBody, dto.ImageUrl);

            var message = dto.ToModel(
                htmlBodyTemplate, recipients.Count, "Sending", adminUserId, adminName);

            LogTrace("SendNowAsync saving initial DB record with Status=Sending", new { recipientsCount = recipients.Count, firstRecipient = recipients.FirstOrDefault() });
            var created = await _repo.CreateAsync(message);
            LogTrace("SendNowAsync saved DB record", new { createdId = created.Id });
            await DispatchEmailsAsync(created, recipients);

            LogTrace("SendNowAsync re-reading updated record from DB");
            var updated = await _repo.GetByIdAsync(created.Id);
            LogTrace("SendNowAsync RETURNING", new { status = updated?.Status, successCount = updated?.SuccessCount, failedCount = updated?.FailedCount, errorMessage = updated?.ErrorMessage });
            return updated!.ToBulkEmailResponseDto();
        }

        // ── SCHEDULE ────────────────────────────────────────────────────────
        public async Task<BulkEmailResponseDto> ScheduleAsync(
            SendBulkEmailDto dto, string adminUserId, string adminName)
        {
            LogTrace("ScheduleAsync ENTERED", new { subject = dto.Subject, targetGroup = dto.TargetGroup, scheduledAt = dto.ScheduledAt });

            if (string.IsNullOrWhiteSpace(dto.Subject))
            {
                LogTrace("ScheduleAsync FAILED: Subject is empty");
                throw new InvalidOperationException("[BulkEmail] Subject is required.");
            }
            if (string.IsNullOrWhiteSpace(dto.HtmlBody))
            {
                LogTrace("ScheduleAsync FAILED: HtmlBody is empty");
                throw new InvalidOperationException("[BulkEmail] HTML body is required.");
            }

            LogTrace("ScheduleAsync calling ResolveRecipientsAsync");
            var recipients = await ResolveRecipientsAsync(dto);
            LogTrace("ScheduleAsync ResolveRecipientsAsync returned", new { recipientCount = recipients.Count });

            if (recipients.Count == 0)
            {
                _logger.LogWarning("[BulkEmail] No recipients resolved for Schedule. Subject: {Subject}", dto.Subject);
                LogTrace("ScheduleAsync EARLY RETURN: 0 recipients, saving with Status=Scheduled. No HTTP request will be made.");
                var empty = dto.ToModel(BuildHtmlTemplate(dto.Subject, dto.HtmlBody, dto.ImageUrl), 0, "Scheduled", adminUserId, adminName);
                return (await _repo.CreateAsync(empty)).ToBulkEmailResponseDto();
            }

            var htmlBodyTemplate = BuildHtmlTemplate(dto.Subject, dto.HtmlBody, dto.ImageUrl);

            var message = dto.ToModel(
                htmlBodyTemplate, recipients.Count, "Scheduled", adminUserId, adminName);

            LogTrace("ScheduleAsync saving DB record", new { recipientsCount = recipients.Count });
            var created = await _repo.CreateAsync(message);
            LogTrace("ScheduleAsync RETURNING", new { createdId = created.Id });
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
            LogTrace("ProcessScheduledEmailsAsync ENTERED");
            var dueMessages = await _repo.GetDueScheduledAsync();
            LogTrace("ProcessScheduledEmailsAsync GetDueScheduledAsync returned", new { count = dueMessages.Count, ids = dueMessages.Select(m => m.Id).ToList() });

            foreach (var message in dueMessages)
            {
                LogTrace("ProcessScheduledEmailsAsync processing message", new { id = message.Id, subject = message.Subject, targetGroup = message.TargetGroup });
                try
                {
                    var recipients = await ResolveRecipientsFromMessageAsync(message);
                    LogTrace("ProcessScheduledEmailsAsync resolved recipients", new { id = message.Id, recipientCount = recipients.Count });
                    message.Status = "Sending";
                    await _repo.UpdateAsync(message);
                    await DispatchEmailsAsync(message, recipients);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to process scheduled BulkEmail {Id}. Marking as Failed.", message.Id);
                    LogTraceException("ProcessScheduledEmailsAsync exception processing message", ex);
                    message.Status = "Failed";
                    message.ErrorMessage = ex.Message;
                    await _repo.UpdateAsync(message);
                }
            }
        }

        // ── PRIVATE: DISPATCH TO BREVO VIA HTTP ─────────────────────────────
        private async Task DispatchEmailsAsync(
            BulkEmailMessage message, List<EmailRecipient> recipients)
        {
            LogTrace("DispatchEmailsAsync ENTERED", new { bulkEmailId = message.Id, recipientCount = recipients.Count });

            if (recipients.Count == 0)
            {
                _logger.LogWarning("[BulkEmail] No recipients to dispatch for BulkEmail {Id}", message.Id);
                LogTrace("DispatchEmailsAsync EARLY RETURN: 0 recipients -> Status hardcoded to Sent. NO HTTP REQUEST TO BREVO.");
                message.Status = "Sent";
                message.SentAt = DateTime.UtcNow;
                await _repo.UpdateAsync(message);
                return;
            }

            if (string.IsNullOrWhiteSpace(_brevo.ApiKey))
            {
                _logger.LogError("[BulkEmail] Brevo API key is not configured. Cannot dispatch BulkEmail {Id}", message.Id);
                LogTrace("DispatchEmailsAsync EARLY RETURN: ApiKey is empty. NO HTTP REQUEST TO BREVO.", new { apiKey = _brevo.ApiKey, apiKeyLength = _brevo.ApiKey?.Length });
                message.Status = "Failed";
                message.ErrorMessage = "Brevo API key is not configured.";
                await _repo.UpdateAsync(message);
                return;
            }
            LogTrace("DispatchEmailsAsync ApiKey check PASSED", new { apiKeyPrefix = _brevo.ApiKey?.Substring(0, Math.Min(20, _brevo.ApiKey?.Length ?? 0)) + "...", apiKeyLength = _brevo.ApiKey?.Length });

            if (string.IsNullOrWhiteSpace(_brevo.SenderEmail))
            {
                _logger.LogError("[BulkEmail] Brevo sender email is not configured. Cannot dispatch BulkEmail {Id}", message.Id);
                LogTrace("DispatchEmailsAsync EARLY RETURN: SenderEmail is empty. NO HTTP REQUEST TO BREVO.");
                message.Status = "Failed";
                message.ErrorMessage = "Brevo sender email is not configured.";
                await _repo.UpdateAsync(message);
                return;
            }
            LogTrace("DispatchEmailsAsync SenderEmail check PASSED", new { senderEmail = _brevo.SenderEmail, senderName = _brevo.SenderName });

            var htmlBody = message.HtmlBody ?? string.Empty;

            var httpClient = _httpClientFactory.CreateClient("BrevoClient");
            LogTrace("DispatchEmailsAsync HttpClient created", new { baseAddress = httpClient.BaseAddress?.ToString() });

            httpClient.DefaultRequestHeaders.Remove("api-key");
            httpClient.DefaultRequestHeaders.Add("api-key", _brevo.ApiKey);
            LogTrace("DispatchEmailsAsync api-key header set on HttpClient");

            try
            {
                int successCount = 0;
                int failedCount = 0;

                var batches = recipients
                    .Select((r, index) => new { r, index })
                    .GroupBy(x => x.index / EmailBatchSize)
                    .Select(g => g.Select(x => x.r).ToList())
                    .ToList();

                LogTrace("DispatchEmailsAsync batches created", new { batchCount = batches.Count, batchSizes = batches.Select(b => b.Count).ToList() });

                foreach (var batch in batches)
                {
                    foreach (var recipient in batch)
                    {
                        try
                        {
                            var personalizedBody = htmlBody
                                .Replace("{{firstName}}", recipient.FirstName ?? "Beloved");

                            var payload = new
                            {
                                sender = new
                                {
                                    name = _brevo.SenderName ?? "Global Flame",
                                    email = _brevo.SenderEmail
                                },
                                to = new[]
                                {
                                    new { email = recipient.Email }
                                },
                                subject = message.Subject ?? "(no subject)",
                                htmlContent = personalizedBody
                            };

                            var json = JsonSerializer.Serialize(payload);
                            var content = new StringContent(
                                json, Encoding.UTF8, "application/json");

                            var targetUrl = $"{(httpClient.BaseAddress?.ToString().TrimEnd('/') ?? "https://api.brevo.com/v3")}/smtp/email";
                            LogTrace("DispatchEmailsAsync ABOUT TO CALL PostAsync", new { url = targetUrl, recipientEmail = recipient.Email, recipientFirstName = recipient.FirstName, bulkEmailId = message.Id });

                            var response = await httpClient.PostAsync("smtp/email", content);
                            LogTrace("DispatchEmailsAsync PostAsync COMPLETED", new { httpStatusCode = (int)response.StatusCode, reasonPhrase = response.ReasonPhrase, isSuccess = response.IsSuccessStatusCode, bulkEmailId = message.Id, recipientEmail = recipient.Email });

                            var responseBody = await response.Content.ReadAsStringAsync();
                            LogTrace("DispatchEmailsAsync response body read", new { bodyLength = responseBody?.Length ?? 0, bodyPreview = responseBody?.Substring(0, Math.Min(500, responseBody?.Length ?? 0)) });

                            if (response.IsSuccessStatusCode)
                            {
                                successCount++;
                                _logger.LogInformation(
                                    "Email sent to {Email} for BulkEmail {Id}",
                                    recipient.Email, message.Id);
                                LogTrace("DispatchEmailsAsync BREVO ACCEPTED email", new { recipientEmail = recipient.Email, statusCode = (int)response.StatusCode, responseBody });
                            }
                            else
                            {
                                failedCount++;
                                _logger.LogError(
                                    "Brevo rejected email to {Email} for BulkEmail {Id}. " +
                                    "Status: {Status}. Response: {Body}",
                                    recipient.Email, message.Id,
                                    response.StatusCode, responseBody);
                                LogTrace("DispatchEmailsAsync BREVO REJECTED email", new { recipientEmail = recipient.Email, statusCode = (int)response.StatusCode, responseBody });
                            }

                            await Task.Delay(100);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "Exception sending to {Email} for BulkEmail {Id}",
                                recipient.Email, message.Id);
                            LogTraceException("DispatchEmailsAsync INNER CATCH: exception during send to recipient", ex);
                            failedCount++;
                        }
                    }

                    await Task.Delay(500);
                }

                message.Status = failedCount == recipients.Count ? "Failed" : "Sent";
                message.SuccessCount = successCount;
                message.FailedCount = failedCount;
                message.SentAt = DateTime.UtcNow;

                LogTrace("DispatchEmailsAsync saving final status", new { status = message.Status, successCount, failedCount, totalRecipients = recipients.Count, errorMessage = message.ErrorMessage });
                await _repo.UpdateAsync(message);

                _logger.LogInformation(
                    "BulkEmail {Id} complete. Sent: {Success}, Failed: {Failed}",
                    message.Id, successCount, failedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Dispatch completely failed for BulkEmail {Id}", message.Id);
                LogTraceException("DispatchEmailsAsync OUTER CATCH: dispatch completely failed", ex);
                message.Status = "Failed";
                message.ErrorMessage = ex.Message;
                await _repo.UpdateAsync(message);
            }
        }

        // ── PRIVATE: RESOLVE RECIPIENTS ─────────────────────────────────────
        private async Task<List<EmailRecipient>> ResolveRecipientsAsync(SendBulkEmailDto dto)
        {
            LogTrace("ResolveRecipientsAsync ENTERED", new { targetGroup = dto.TargetGroup, customEmails = dto.CustomEmails });

            if (dto.TargetGroup == "Custom")
            {
                if (string.IsNullOrWhiteSpace(dto.CustomEmails))
                {
                    LogTrace("ResolveRecipientsAsync Custom target but CustomEmails is empty -> returning 0 recipients");
                    return new List<EmailRecipient>();
                }

                var parsed = dto.CustomEmails
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Distinct()
                    .Select(e => new EmailRecipient(e, "Beloved"))
                    .ToList();

                LogTrace("ResolveRecipientsAsync parsed custom emails", new { count = parsed.Count, emails = parsed.Select(r => r.Email).ToList() });
                return parsed;
            }

            var query = _context.Users.Where(u => u.EmailConfirmed);
            LogTrace("ResolveRecipientsAsync base query: EmailConfirmed users");

            if (dto.TargetGroup == "Ministry")
            {
                query = query.Where(u => u.Module == "Ministry");
                LogTrace("ResolveRecipientsAsync filtered to Ministry");
            }
            else if (dto.TargetGroup == "Youth")
            {
                query = query.Where(u => u.Module == "Youth");
                LogTrace("ResolveRecipientsAsync filtered to Youth");
            }
            else
            {
                LogTrace("ResolveRecipientsAsync target group is All (no module filter)");
            }

            var result = await query
                .Where(u => !string.IsNullOrWhiteSpace(u.Email))
                .Select(u => new EmailRecipient(u.Email!, u.FirstName))
                .ToListAsync();

            LogTrace("ResolveRecipientsAsync query executed", new { count = result.Count, firstFew = result.Take(5).Select(r => new { r.Email, r.FirstName }).ToList() });
            return result;
        }

        private async Task<List<EmailRecipient>> ResolveRecipientsFromMessageAsync(
            BulkEmailMessage message)
        {
            LogTrace("ResolveRecipientsFromMessageAsync ENTERED", new { bulkEmailId = message.Id, targetGroup = message.TargetGroup });

            if (message.TargetGroup == "Custom" &&
                !string.IsNullOrWhiteSpace(message.CustomEmailsJson))
            {
                var parsed = message.CustomEmailsJson
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .Select(e => new EmailRecipient(e, "Member"))
                    .ToList();
                LogTrace("ResolveRecipientsFromMessageAsync parsed custom emails", new { count = parsed.Count });
                return parsed;
            }

            var query = _context.Users.Where(u => u.EmailConfirmed);

            if (message.TargetGroup == "Ministry")
                query = query.Where(u => u.Module == "Ministry");
            else if (message.TargetGroup == "Youth")
                query = query.Where(u => u.Module == "Youth");

            var result = await query
                .Where(u => !string.IsNullOrWhiteSpace(u.Email))
                .Select(u => new EmailRecipient(u.Email!, u.FirstName))
                .ToListAsync();

            LogTrace("ResolveRecipientsFromMessageAsync query executed", new { count = result.Count });
            return result;
        }

        private static string BuildHtmlTemplate(string subject, string body, string? imageUrl)
        {
            var encodedSubject = HtmlEncoder.Default.Encode(subject ?? "");

            var encodedBody = HtmlEncoder.Default.Encode(body ?? "")
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