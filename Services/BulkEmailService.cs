using System.Net.Http.Headers;
using System.Text;
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
        private readonly IBulkEmailRepository _repo;
        private readonly AppDbContext _context;
        private readonly BrevoSettings _brevo;
        private readonly HttpClient _httpClient;
        private readonly ILogger<BulkEmailService> _logger;

        public BulkEmailService(IBulkEmailRepository repo, AppDbContext context, IOptions<BrevoSettings> brevoOptions, HttpClient httpClient, ILogger<BulkEmailService> logger)
        {
            _repo = repo;
            _context = context;
            _brevo = brevoOptions.Value;
            _httpClient = httpClient;
            _logger = logger;

            // Set Brevo auth header once
            _httpClient.BaseAddress = new Uri("https://api.brevo.com/v3/");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("api-key", _brevo.ApiKey);
            _httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // SEND NOW 
        public async Task<BulkEmailResponseDto> SendNowAsync(
            SendBulkEmailDto dto, string adminUserId, string adminName)
        {
            var recipients = await ResolveRecipientsAsync(dto);
            var htmlBody = BuildHtmlTemplate(dto.Subject, dto.HtmlBody);

            var message = dto.ToModel(
                htmlBody, recipients.Count, "Sending", adminUserId, adminName);

            var created = await _repo.CreateAsync(message);

            // Dispatch synchronously — no fire and forget
            // This avoids DbContext disposal issues in background threads
            await DispatchEmailsAsync(created, recipients);

            return created.ToBulkEmailResponseDto();
        }

        // SCHEDULE 
        public async Task<BulkEmailResponseDto> ScheduleAsync(
            SendBulkEmailDto dto, string adminUserId, string adminName)
        {
            var recipients = await ResolveRecipientsAsync(dto);
            var htmlBody = BuildHtmlTemplate(dto.Subject, dto.HtmlBody);

            var message = dto.ToModel(
                htmlBody, recipients.Count, "Scheduled", adminUserId, adminName);

            var created = await _repo.CreateAsync(message);
            return created.ToBulkEmailResponseDto();
        }

        // GET HISTORY 
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

        // GET STATS 
        public async Task<BulkEmailStatsDto> GetStatsAsync()
        {
            return await _repo.GetStatsAsync();
        }

        // CANCEL SCHEDULED 
        public async Task<bool> CancelScheduledAsync(int id)
        {
            return await _repo.CancelAsync(id);
        }

        // PROCESS SCHEDULED (called by background service) 
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

        //  PRIVATE: DISPATCH TO BREVO VIA HTTP 
        private async Task DispatchEmailsAsync(
            BulkEmailMessage message,
            List<string> recipients)
        {
            try
            {
                int successCount = 0;
                int failedCount = 0;

                // Batch into groups of 50
                var batches = recipients
                    .Select((email, index) => new { email, index })
                    .GroupBy(x => x.index / 50)
                    .Select(g => g.Select(x => x.email).ToList())
                    .ToList();

                foreach (var batch in batches)
                {
                    try
                    {
                        var payload = new
                        {
                            sender = new
                            {
                                name = _brevo.SenderName,
                                email = _brevo.SenderEmail
                            },
                            to = batch.Select(e => new { email = e }).ToArray(),
                            subject = message.Subject,
                            htmlContent = message.HtmlBody
                        };

                        var json = JsonSerializer.Serialize(payload);
                        var content = new StringContent(
                            json, Encoding.UTF8, "application/json");

                        var response = await _httpClient
                            .PostAsync("smtp/email", content);

                        var responseBody = await response.Content
                            .ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            successCount += batch.Count;
                            _logger.LogInformation(
                                "Batch sent successfully for BulkEmail {Id}. " +
                                "Recipients: {Count}", message.Id, batch.Count);
                        }
                        else
                        {
                            failedCount += batch.Count;
                            _logger.LogError(
                                "Brevo rejected batch for BulkEmail {Id}. " +
                                "Status: {Status}. Response: {Body}",
                                message.Id, response.StatusCode, responseBody);
                        }

                        // Polite delay between batches
                        await Task.Delay(500);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Batch exception for BulkEmail {Id}", message.Id);
                        failedCount += batch.Count;
                    }
                }

                message.Status = failedCount == recipients.Count
                    ? "Failed" : "Sent";
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

        // PRIVATE: RESOLVE RECIPIENTS 
        private async Task<List<string>> ResolveRecipientsAsync(SendBulkEmailDto dto)
        {
            if (dto.TargetGroup == "Custom")
            {
                if (string.IsNullOrWhiteSpace(dto.CustomEmails))
                    return new List<string>();

                return dto.CustomEmails
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Distinct()
                    .ToList();
            }

            var query = _context.Users.Where(u => u.EmailConfirmed);

            if (dto.TargetGroup == "Ministry")
                query = query.Where(u => u.Module == "Ministry");
            else if (dto.TargetGroup == "Youth")
                query = query.Where(u => u.Module == "Youth");

            return await query.Select(u => u.Email!).Where(e => !string.IsNullOrWhiteSpace(e)).ToListAsync();
        }

        private async Task<List<string>> ResolveRecipientsFromMessageAsync(BulkEmailMessage message)
        {
            if (message.TargetGroup == "Custom" &&
                !string.IsNullOrWhiteSpace(message.CustomEmailsJson))
            {
                return message.CustomEmailsJson.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim()).ToList();
            }

            var query = _context.Users.Where(u => u.EmailConfirmed);

            if (message.TargetGroup == "Ministry")
                query = query.Where(u => u.Module == "Ministry");
            else if (message.TargetGroup == "Youth")
                query = query.Where(u => u.Module == "Youth");

            return await query.Select(u => u.Email!).Where(e => !string.IsNullOrWhiteSpace(e)).ToListAsync();
        }

        // PRIVATE: HTML TEMPLATE 
        private static string BuildHtmlTemplate(string subject, string body)
        {
            return $"""
            <!DOCTYPE html>
            <html>
            <head>
              <meta charset="UTF-8"/>
              <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
              <title>{subject}</title>
            </head>
            <body style="margin:0;padding:0;background:#f4f4f7;font-family:Arial,sans-serif;">
              <table width="100%" cellpadding="0" cellspacing="0"
                     style="background:#f4f4f7;padding:40px 0;">
                <tr>
                  <td align="center">
                    <table width="600" cellpadding="0" cellspacing="0"
                           style="background:#ffffff;border-radius:12px;
                                  overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,0.08);">
                      <tr>
                        <td style="background:linear-gradient(135deg,#a21caf,#7c3aed);
                                   padding:32px 40px;text-align:center;">
                          <div style="font-size:32px;margin-bottom:8px;">🔥</div>
                          <h1 style="color:#ffffff;margin:0;font-size:22px;
                                     font-weight:bold;letter-spacing:1px;">
                            GLOBAL FLAME MINISTRIES
                          </h1>
                          <p style="color:rgba(255,255,255,0.8);margin:4px 0 0;font-size:13px;">
                            Empowering the Next Generation
                          </p>
                        </td>
                      </tr>
                      <tr>
                        <td style="background:#faf5ff;padding:20px 40px;
                                   border-bottom:2px solid #e9d5ff;">
                          <h2 style="color:#7c3aed;margin:0;font-size:18px;">
                            {subject}
                          </h2>
                        </td>
                      </tr>
                      <tr>
                        <td style="padding:32px 40px;color:#374151;
                                   font-size:15px;line-height:1.7;">
                          {body.Replace("\n", "<br/>")}
                        </td>
                      </tr>
                      <tr>
                        <td style="background:#f9fafb;padding:24px 40px;
                                   text-align:center;border-top:1px solid #e5e7eb;">
                          <p style="color:#9ca3af;font-size:12px;margin:0 0 8px;">
                            You are receiving this email because you are a registered
                            member of Global Flame Ministries.
                          </p>
                          <p style="color:#9ca3af;font-size:12px;margin:0;">
                            © {DateTime.UtcNow.Year} Global Flame Ministries.
                            All rights reserved.
                          </p>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </body>
            </html>
            """;
        }
    }
}