using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.PrayerRequest;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Interfaces.Email;
using GlobalFlameMinistry.API.Mappers;

namespace GlobalFlameMinistry.API.Services
{
    public class PrayerRequestService : IPrayerRequestService
    {
        private readonly IPrayerRequestRepository _prayerRepo;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        public PrayerRequestService(IPrayerRequestRepository prayerRepo, IEmailSender emailSender, IConfiguration config)
        {
            _prayerRepo = prayerRepo;
            _emailSender = emailSender;
            _config = config;
        }

        public async Task<PrayerRequestResponseDto> CreateAsync(CreatePrayerDto dto, string? name, string? email, string? appUserId)
        {
            var request = dto.ToPrayerFromCreateDto(name!, email!, appUserId);
            var created = await _prayerRepo.CreateAsync(request);

            try
            {
                await SendConfirmationEmailAsync(created.Email, created.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PrayerRequestService] Confirmation email failed: {ex.Message}");
            }

            var churchInbox = _config["PrayerInboxEmail"];
            if (!string.IsNullOrWhiteSpace(churchInbox))
            {
                try
                {
                    await SendChurchInboxNotificationAsync(
                        churchInbox,
                        created.Name,
                        created.Email,
                        created.PhoneNumber,
                        created.PreferredContact,
                        created.Topic,
                        created.Content,
                        created.Attachment);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PrayerRequestService] Church inbox email failed: {ex.Message}");
                }
            }

            return created.ToPrayerResponseDto();
        }

        public async Task<PagedResult<PrayerRequestResponseDto>> GetAllAsync(PrayerRequestQueryObject query)
        {
            var requests = await _prayerRepo.GetAllAsync(query);
            var totalCount = await _prayerRepo.GetCountAsync(query);

            return new PagedResult<PrayerRequestResponseDto>
            {
                Items = requests.ToDtoList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<PrayerRequestResponseDto?> GetByIdAsync(int id)
        {
            var request = await _prayerRepo.GetByIdAsync(id);

            return request?.ToPrayerResponseDto();
        }

        public async Task<PrayerRequestResponseDto?> GetByTokenAsync(string token)
        {
            var request = await _prayerRepo.GetByTokenAsync(token);

            return request?.ToPrayerResponseDto();
        }

        public async Task<PrayerRequestResponseDto?> MarkAsAttendedAsync(int id, UpdatePrayerRequestDto dto)
        {
            var updated = await _prayerRepo.UpdateAsync(id, dto);
            return updated?.ToPrayerResponseDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _prayerRepo.DeleteAsync(id);
        }

        // EMAILS 
        private async Task SendConfirmationEmailAsync(string toEmail, string fullName)
        {
            var firstName = fullName.Split(' ')[0];
            var subject = "Prayer Request Received — Global Flame Ministry";
            var body = $@"
        <div style='font-family: Georgia, serif; max-width: 600px; margin: auto;
            padding: 40px; background: #ffffff;'>
          <h2 style='color: #0f172a;'>Dear {firstName},</h2>
          <p style='color: #475569; font-size: 16px; line-height: 1.7; text-align: justify;'>
            We have received your prayer request and our pastoral team will be
            interceding on your behalf. You can expect to hear from us within
            <strong>48 hours</strong>.
          </p>
          <p style='color: #475569; font-size: 16px; line-height: 1.7; text-align: justify;'>
            Be assured that everything shared is treated with the utmost
            confidentiality and care.
          </p>
          <blockquote style='border-left: 4px solid #a855f7; margin: 24px 0;
              padding: 12px 20px; background: #faf5ff; color: #7c3aed;
              font-style: italic;'>
            &#8220;The prayer of a righteous person is powerful and effective.&#8221;
            &#8212; James 5:16
          </blockquote>
          <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 32px 0;' />
          <p style='color: #94a3b8; font-size: 13px;'>
            Global Flame Ministry &middot; Jos, Plateau State, Nigeria
          </p>
        </div>";

            await _emailSender.SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendChurchInboxNotificationAsync(string toEmail, string requesterName, string requesterEmail, string? requesterPhone, string preferredContact, string? topic, string content, string? attachment)
        {
            var subject = $" New Prayer Request — {requesterName}";

            var topicRow = !string.IsNullOrWhiteSpace(topic)
            ? $@"<tr>
               <td style='padding: 10px; border: 1px solid #e2e8f0;
                   font-weight: bold; background: #f8fafc;'>Topic</td>
               <td style='padding: 10px; border: 1px solid #e2e8f0;'>
                 <strong>{topic}</strong>
               </td>
             </tr>"
            : string.Empty;

            var attachmentRow = !string.IsNullOrWhiteSpace(attachment)
                ? $@"<tr>
               <td style='padding: 10px; border: 1px solid #e2e8f0;
                   font-weight: bold; background: #f8fafc;'>Attachment</td>
               <td style='padding: 10px; border: 1px solid #e2e8f0;'>
                 <a href='{attachment}'>View attachment</a>
               </td>
             </tr>"
                : string.Empty;

            var body = $@"
        <div style='font-family: Georgia, serif; max-width: 600px; margin: auto;
            padding: 40px; background: #ffffff;'>
          <h2 style='color: #0f172a; border-bottom: 2px solid #a855f7;
              padding-bottom: 12px; margin-bottom: 24px;'>
            New Prayer Request
          </h2>
          <p style='color: #475569; font-size: 15px; line-height: 1.6;'>
            A new prayer request has been submitted via the website.
            Please reach out to the person using the contact details below.
          </p>
          <table style='width: 100%; border-collapse: collapse; margin: 24px 0;'>
            <tr>
              <td style='padding: 10px; border: 1px solid #e2e8f0;
                  font-weight: bold; background: #f8fafc; width: 35%;'>Name</td>
              <td style='padding: 10px; border: 1px solid #e2e8f0;'>{requesterName}</td>
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
            {attachmentRow}
          </table>
          <p style='color: #475569; font-weight: bold; margin-bottom: 8px;'>
            Prayer Request:
          </p>
          <p style='color: #475569; background: #f8fafc; padding: 16px;
              border-left: 4px solid #a855f7; line-height: 1.7;
              white-space: pre-wrap;'>
            {content}
          </p>
          <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 32px 0;' />
          <p style='color: #94a3b8; font-size: 13px;'>
            Global Flame Ministry &middot; Jos, Plateau State, Nigeria<br/>
            Reply directly to
            <a href='mailto:{requesterEmail}'>{requesterEmail}</a> to contact the requester.
          </p>
        </div>";

            await _emailSender.SendEmailAsync(toEmail, subject, body);
        }
    }
}