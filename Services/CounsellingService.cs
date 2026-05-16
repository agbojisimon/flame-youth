using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Counselling;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces.Counselling;
using GlobalFlameMinistry.API.Interfaces.Email;
using GlobalFlameMinistry.API.Mappers;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Services
{
  public class CounsellingService : ICounsellingService
  {
    private readonly ICounsellingRepository _repo;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _config;
    private readonly ILogger<CounsellingService> _logger;

    public CounsellingService(ICounsellingRepository repo, IEmailSender emailSender, IConfiguration config, ILogger<CounsellingService> logger)
    {
      _repo = repo;
      _emailSender = emailSender;
      _config = config;
      _logger = logger;
    }

    public async Task<CounsellingResponseDto> CreateAsync(CreateCounsellingRequestDto dto, string? appUserId)
    {
      var model = dto.ToModel(appUserId);
      var created = await _repo.CreateAsync(model);

      try
      {
        await SendConfirmationEmailAsync(
            created.Email, created.FullName, created.Topic);
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex,
            "[CounsellingService] Confirmation email failed for {Email}",
            created.Email);
      }

      var churchInbox = _config["CounsellingInboxEmail"];
      if (!string.IsNullOrWhiteSpace(churchInbox))
      {
        try
        {
          await SendChurchInboxNotificationAsync(
              churchInbox,
              created.FullName,
              created.Email,
              created.PhoneNumber,
              created.Topic,
              created.Message,
              created.PreferredContact);
        }
        catch (Exception ex)
        {
          _logger.LogWarning(ex,
              "[CounsellingService] Church inbox notification failed for request by {Email}", created.Email);
        }
      }
      else
      {
        _logger.LogWarning(
            "[CounsellingService] No church inbox email configured, skipping notification for request by {Email}",
            created.Email);
      }

      return created.ToResponseDto();
    }

    public async Task<PagedResult<CounsellingResponseDto>> GetAllAsync(
        CounsellingQueryObject query)
    {
      var items = await _repo.GetAllAsync(query);
      var total = await _repo.GetCountAsync(query);

      return new PagedResult<CounsellingResponseDto>
      {
        Items = items.ToDtoList(),
        TotalCount = total,
        PageNumber = query.PageNumber,
        PageSize = query.PageSize
      };
    }

    public async Task<CounsellingResponseDto?> GetByIdAsync(int id)
    {
      var request = await _repo.GetByIdAsync(id);
      return request?.ToResponseDto();
    }

    public async Task<CounsellingResponseDto?> UpdateStatusAsync(int id, CounsellingStatus status)
    {
      var updated = await _repo.UpdateStatusAsync(id, status);
      return updated?.ToResponseDto();
    }

    public async Task<bool> DeleteAsync(int id)
    {
      return await _repo.DeleteAsync(id);
    }

    // EMAILS 
    private async Task SendConfirmationEmailAsync(
        string toEmail, string fullName, string topic)
    {
      var firstName = fullName.Split(' ')[0];
      var subject = "Counselling Request Received — Global Flame";
      var body = $@"
                <div style='font-family: Georgia, serif; max-width: 600px; margin: auto;
                    padding: 40px; background: #ffffff;'>
                  <h2 style='color: #0f172a;'>Dear {firstName},</h2>
                  <p style='color: #475569; font-size: 16px; line-height: 1.7; text-align: justify;'>
                    Thank you for reaching out. We have received your counselling request
                    regarding <strong>{topic}</strong> and a member of our pastoral team
                    will be in touch with you shortly.
                  </p>
                  <p style='color: #475569; font-size: 16px; line-height: 1.7; text-align: justify;'>
                    Please be assured that all matters shared are treated with the utmost
                    confidentiality and care.
                  </p>
                  <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 32px 0;' />
                  <p style='color: #94a3b8; font-size: 13px;'>
                    Global Flame · Jos, Plateau State, Nigeria
                  </p>
                </div>";

      await _emailSender.SendEmailAsync(toEmail, subject, body);
    }

    private async Task SendChurchInboxNotificationAsync(string toEmail, string requesterName, string requesterEmail, string? requesterPhone, string topic, string message, string preferredContact)   // removed int requestId
    {
      var subject = $"🆕 New Counselling Request — {requesterName} ({topic})";
      var body = $@"
        <div style='font-family: Georgia, serif; max-width: 600px; margin: auto;
            padding: 40px; background: #ffffff;'>
          <h2 style='color: #0f172a; border-bottom: 2px solid #a855f7;
              padding-bottom: 12px; margin-bottom: 24px;'>
            New Counselling Request
          </h2>
          <p style='color: #475569; font-size: 15px; line-height: 1.6;'>
            A new counselling request has been submitted via the website.
            Please log in to the admin panel to update the status or
            respond directly using the contact details below.
          </p>
          <table style='width: 100%; border-collapse: collapse; margin: 24px 0;'>
            <tr>
              <td style='padding: 10px; border: 1px solid #e2e8f0;
                  font-weight: bold; background: #f8fafc; width: 35%;'>Name</td>
              <td style='padding: 10px; border: 1px solid #e2e8f0;'>{requesterName}</td>
            </tr>
            <tr>
              <td style='padding: 10px; border: 1px solid #e2e8f0;
                  font-weight: bold; background: #f8fafc;'>Topic</td>
              <td style='padding: 10px; border: 1px solid #e2e8f0;'>
                <strong>{topic}</strong>
              </td>
            </tr>
            <tr>
              <td style='padding: 10px; border: 1px solid #e2e8f0;
                  font-weight: bold; background: #f8fafc;'>Email</td>
              <td style='padding: 10px; border: 1px solid #e2e8f0;'>
                <a href='mailto:{requesterEmail}'>{requesterEmail}</a>
              </td>
            </tr>
            <tr>
              <td style='padding: 10px; border: 1px solid #e2e8f0;
                  font-weight: bold; background: #f8fafc;'>Phone</td>
              <td style='padding: 10px; border: 1px solid #e2e8f0;'>
                {requesterPhone ?? "<em>Not provided</em>"}
              </td>
            </tr>
            <tr>
              <td style='padding: 10px; border: 1px solid #e2e8f0;
                  font-weight: bold; background: #f8fafc;'>Preferred Contact</td>
              <td style='padding: 10px; border: 1px solid #e2e8f0;'>{preferredContact}</td>
            </tr>
          </table>
          <p style='color: #475569; font-weight: bold; margin-bottom: 8px;'>
            Message:
          </p>
          <p style='color: #475569; background: #f8fafc; padding: 16px;
              border-left: 4px solid #a855f7; line-height: 1.7;
              white-space: pre-wrap;'>
            {message}
          </p>
          <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 32px 0;' />
          <p style='color: #94a3b8; font-size: 13px;'>
            Global Flame &middot; Jos, Plateau State, Nigeria<br/>
            Reply directly to
            <a href='mailto:{requesterEmail}'>{requesterEmail}</a> to contact the requester.
          </p>
        </div>";

      await _emailSender.SendEmailAsync(toEmail, subject, body);
    }
  }
}