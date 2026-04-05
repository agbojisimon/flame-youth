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

        public CounsellingService(ICounsellingRepository repo, IEmailSender emailSender)
        {
            _repo = repo;
            _emailSender = emailSender;
        }

        public async Task<CounsellingResponseDto> CreateAsync(
            CreateCounsellingRequestDto dto, string? appUserId)
        {
            var model = dto.ToModel(appUserId);
            var created = await _repo.CreateAsync(model);

            // Fire confirmation email — don't let a failure block the response
            try
            {
                await SendConfirmationEmailAsync(created.Email, created.FullName, created.Topic);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CounsellingService] Email failed: {ex.Message}");
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

        public async Task<CounsellingResponseDto?> AssignAsync(int id, AssignCounsellorDto dto)
        {
            var updated = await _repo.AssignAsync(id, dto);

            if (updated is null)
                return null;

            // Notify the assigned counsellor by email
            try
            {
                await SendAssignmentEmailAsync(
                    updated.AssignedToEmail!,
                    updated.AssignedTo!,
                    updated.FullName,
                    updated.Topic,
                    updated.Email,
                    updated.PhoneNumber,
                    updated.PreferredContact,
                    updated.Message
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CounsellingService] Assignment email failed: {ex.Message}");
            }

            return updated.ToResponseDto();
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
            var subject = "Counselling Request Received — Global Flame Ministries";
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
                    Global Flame Ministry · Jos, Plateau State, Nigeria
                  </p>
                </div>";

            await _emailSender.SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendAssignmentEmailAsync(string toEmail, string counsellorName, string requesterName, string topic, string requesterEmail, string? requesterPhone, string preferredContact, string message)
        {
            var subject = $"New Counselling Assignment — {requesterName}";
            var body = $@"
                <div style='font-family: Georgia, serif; max-width: 600px; margin: auto;
                    padding: 40px; background: #ffffff;'>
                  <h2 style='color: #0f172a;'>Dear {counsellorName},</h2>
                  <p style='color: #475569; font-size: 16px; line-height: 1.7; text-align: justify;'>
                    You have been assigned a new counselling request. Here are the details:
                  </p>
                  <table style='width: 100%; border-collapse: collapse; margin: 24px 0;'>
                    <tr>
                      <td style='padding: 10px; border: 1px solid #e2e8f0;
                          font-weight: bold; background: #f8fafc;'>Name</td>
                      <td style='padding: 10px; border: 1px solid #e2e8f0;'>{requesterName}</td>
                    </tr>
                    <tr>
                      <td style='padding: 10px; border: 1px solid #e2e8f0;
                          font-weight: bold; background: #f8fafc;'>Topic</td>
                      <td style='padding: 10px; border: 1px solid #e2e8f0;'>{topic}</td>
                    </tr>
                    <tr>
                      <td style='padding: 10px; border: 1px solid #e2e8f0;
                          font-weight: bold; background: #f8fafc;'>Email</td>
                      <td style='padding: 10px; border: 1px solid #e2e8f0;'>{requesterEmail}</td>
                    </tr>
                    <tr>
                      <td style='padding: 10px; border: 1px solid #e2e8f0;
                          font-weight: bold; background: #f8fafc;'>Phone</td>
                      <td style='padding: 10px; border: 1px solid #e2e8f0;'>
                        {requesterPhone ?? "Not provided"}</td>
                    </tr>
                    <tr>
                      <td style='padding: 10px; border: 1px solid #e2e8f0;
                          font-weight: bold; background: #f8fafc;'>Preferred Contact</td>
                      <td style='padding: 10px; border: 1px solid #e2e8f0;'>{preferredContact}</td>
                    </tr>
                  </table>
                  <p style='color: #475569; font-weight: bold;'>Message:</p>
                  <p style='color: #475569; background: #f8fafc; padding: 16px;
                      border-left: 4px solid #a855f7; line-height: 1.7;'>{message}</p>
                  <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 32px 0;' />
                  <p style='color: #94a3b8; font-size: 13px;'>
                    Global Flame Ministries · Jos, Plateau State, Nigeria
                  </p>
                </div>";

            await _emailSender.SendEmailAsync(toEmail, subject, body);
        }
    }
}