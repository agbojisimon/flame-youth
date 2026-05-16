using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Testimony;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Interfaces.Email;
using GlobalFlameMinistry.API.Mappers;

namespace GlobalFlameMinistry.API.Services
{
    public class TestimonyService : ITestimonyService
    {
        private readonly ITestimonyRepository _testimonyRepo;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<TestimonyService> _logger;

        public TestimonyService(ITestimonyRepository testimonyRepo, IEmailSender emailSender, ILogger<TestimonyService> logger)
        {
            _testimonyRepo = testimonyRepo;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task<TestimonyResponseDto> CreateAsync(CreateTestimonyDto createDto, string? name, string? appUserId)
        {
            var testimony = createDto.ToTestimonyFromCreateDto(name, appUserId);

            var created = await _testimonyRepo.CreateAsync(testimony);

            try
            {
                await SendAdminNotificationAsync(created.FullName, created.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[TestimonyService] Failed to send admin notification email");
            }

            return created.ToTestimonyResponseDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _testimonyRepo.DeleteAsync(id);
        }

        public async Task<PagedResult<TestimonyResponseDto>> GetAllAsync(TestimonyQueryObject query)
        {
            var testimonies = await _testimonyRepo.GetAllAsync(query);
            var totalCount = await _testimonyRepo.GetAllCountAsync(query);

            return new PagedResult<TestimonyResponseDto>
            {
                Items = testimonies.ToDtoList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<PagedResult<TestimonyResponseDto>> GetApprovedAsync(TestimonyQueryObject query)
        {
            var testimonies = await _testimonyRepo.GetApprovedAsync(query);
            var totalCount = await _testimonyRepo.GetApprovedCountAsync(query);

            return new PagedResult<TestimonyResponseDto>
            {
                Items = testimonies.ToDtoList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<TestimonyResponseDto?> GetByIdAsync(int id)
        {
            var testimony = await _testimonyRepo.GetByIdAsync(id);

            if (testimony is null)
                return null;

            return testimony.ToTestimonyResponseDto();
        }

        public async Task<TestimonyResponseDto?> UpdateStatusAsync(int id, UpdateTestimonyDto updateDto)
        {
            var updated = await _testimonyRepo.UpdateStatusAsync(id, updateDto);

            if (updated is null)
                return null;

            return updated.ToTestimonyResponseDto();
        }

        private async Task SendAdminNotificationAsync(string? submitterName, string? content)
        {
            var subject = "New Testimony Submitted";

            var body = $"""
                <div style="font-family: Georgia, serif; max-width: 600px; margin: auto; padding: 40px; background: #ffffff;">
                  
                  <h2 style="color: #0f172a; font-size: 20px; margin-bottom: 16px;">
                    New Testimony Submission
                  </h2>

                  <p style="color: #475569; font-size: 15px; line-height: 1.6; margin-bottom: 24px;">
                    A new testimony has been submitted and is awaiting review.
                  </p>

                  <div style="background: #f8fafc; padding: 16px; border-left: 4px solid #a855f7; margin-bottom: 24px;">
                    <p style="color: #475569; font-size: 14px; margin: 0 0 12px 0;">
                      <strong>Submitter Name:</strong><br/>
                      {(string.IsNullOrWhiteSpace(submitterName) ? "<em>Not provided</em>" : submitterName)}
                    </p>
                    <p style="color: #475569; font-size: 14px; margin: 0;">
                      <strong>Testimony Content:</strong><br/>
                      {(string.IsNullOrWhiteSpace(content) ? "<em>No content</em>" : content)}
                    </p>
                  </div>

                  <p style="color: #94a3b8; font-size: 13px;">
                    Global Flame Ministry · Jos, Plateau State, Nigeria
                  </p>
                </div>
                """;

            await _emailSender.SendEmailAsync("info@globalflameministry.org", subject, body);
        }
    }
}