using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Testimony;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Models;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Interfaces.Email;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Hybrid;

namespace GlobalFlameMinistry.API.Services
{
    public class TestimonyService : ITestimonyService
    {
        private readonly ITestimonyRepository _testimonyRepo;
        private readonly IEmailSender _emailSender;
        private readonly HybridCache _cache;
        private readonly ILogger<TestimonyService> _logger;

        public TestimonyService(ITestimonyRepository testimonyRepo, IEmailSender emailSender, HybridCache cache, ILogger<TestimonyService> logger)
        {
            _testimonyRepo = testimonyRepo;
            _emailSender = emailSender;
            _cache = cache;
            _logger = logger;
        }

        public async Task<TestimonyResponseDto> CreateAsync(CreateTestimonyDto createDto, string? name, string? appUserId)
        {
            var testimony = createDto.ToTestimonyFromCreateDto(name, appUserId);

            var created = await _testimonyRepo.CreateAsync(testimony);

            try
            {
                await SendAdminNotificationAsync(created);
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
            var cacheKey = string.Format(CacheKeys.TestimoniesApproved, query.PageNumber, query.PageSize);

            async ValueTask<PagedResult<TestimonyResponseDto>> Factory(CancellationToken ct)
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

            try
            {
                return await _cache.GetOrCreateAsync(cacheKey, Factory, tags: [CacheKeys.TagTestimonies], cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB for cache key {CacheKey}", cacheKey);
                return await Factory(CancellationToken.None);
            }
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

            if (updated is not null)
            {
                try
                {
                    await _cache.RemoveByTagAsync(CacheKeys.TagTestimonies, CancellationToken.None);
                    _logger.LogInformation("[TestimonyService] Updated testimony ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagTestimonies);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[TestimonyService] Redis unavailable, cache invalidation skipped for {Tag}", CacheKeys.TagTestimonies);
                }
            }

            return updated?.ToTestimonyResponseDto();
        }

        private async Task SendAdminNotificationAsync(Testimony testimony)
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
                      {(string.IsNullOrWhiteSpace(testimony.FullName) ? "<em>Not provided</em>" : testimony.FullName)}
                    </p>
                    <p style="color: #475569; font-size: 14px; margin: 0 0 12px 0;">
                      <strong>Email Address:</strong><br/>
                      {(string.IsNullOrWhiteSpace(testimony.Email) ? "<em>Not provided</em>" : testimony.Email)}
                    </p>
                    <p style="color: #475569; font-size: 14px; margin: 0 0 12px 0;">
                      <strong>Phone Number:</strong><br/>
                      {(string.IsNullOrWhiteSpace(testimony.PhoneNumber) ? "<em>Not provided</em>" : testimony.PhoneNumber)}
                    </p>
                    <p style="color: #475569; font-size: 14px; margin: 0;">
                      <strong>Testimony Content:</strong><br/>
                      {(string.IsNullOrWhiteSpace(testimony.Content) ? "<em>No content</em>" : testimony.Content)}
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
