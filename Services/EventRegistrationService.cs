using System.Net;
using GlobalFlameMinistry.API.DTOs.Event;
using GlobalFlameMinistry.API.Exceptions;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Interfaces.Email;
using GlobalFlameMinistry.API.Models;
using Microsoft.Extensions.Caching.Hybrid;

namespace GlobalFlameMinistry.API.Services
{
    public class EventRegistrationService : IEventRegistrationService
    {
        private readonly IEventRegistrationRepository _repository;
        private readonly IEmailSender _emailSender;
        private readonly HybridCache _cache;
        private readonly ILogger<EventRegistrationService> _logger;

        public EventRegistrationService(
            IEventRegistrationRepository repository,
            IEmailSender emailSender,
            HybridCache cache,
            ILogger<EventRegistrationService> logger)
        {
            _repository = repository;
            _emailSender = emailSender;
            _cache = cache;
            _logger = logger;
        }

        public async Task<EventRegistrationResponseDto> RegisterAsync(
            int eventId,
            RegisterForEventDto dto,
            string eventTitle,
            DateTime startDate,
            DateTime endDate,
            string location,
            string? appUserId = null)
        {
            // Normalize the email before any comparison or insert
            dto.Email = (dto.Email ?? string.Empty).Trim().ToLowerInvariant();

            // Fast-path hint; the DB unique index is the real authority
            if (await _repository.IsAlreadyRegisteredAsync(eventId, dto.Email))
                throw new EventRegistrationConflictException(
                    "This email is already registered for this event.");

            // Save registration (atomic seat claim + duplicate detection)
            var registration = new EventRegistration
            {
                EventId = eventId,
                AppUserId = appUserId,
                FullName = dto.FullName?.Trim() ?? string.Empty,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber?.Trim(),
                RegisteredAt = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(registration);

            await InvalidateEventsCacheAsync(created.Id);

            // Send confirmation email out-of-band from the HTTP response
            try
            {
                await SendConfirmationEmailAsync(
                    dto, eventTitle, startDate, endDate, location);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[EventRegistration] Confirmation email failed for registration {RegistrationId} to {Email}.",
                    created.Id,
                    created.Email);
            }

            return new EventRegistrationResponseDto
            {
                Id = created.Id,
                EventId = created.EventId,
                EventTitle = eventTitle,
                FullName = created.FullName,
                Email = created.Email,
                PhoneNumber = created.PhoneNumber,
                RegisteredAt = created.RegisteredAt
            };
        }

        public async Task<List<EventRegistrationResponseDto>> GetByEventIdAsync(
            int eventId)
        {
            var registrations = await _repository.GetByEventIdAsync(eventId);
            return registrations.Select(r => new EventRegistrationResponseDto
            {
                Id = r.Id,
                EventId = r.EventId,
                FullName = r.FullName,
                Email = r.Email,
                PhoneNumber = r.PhoneNumber,
                RegisteredAt = r.RegisteredAt
            }).ToList();
        }

        public async Task<int> GetCountByEventIdAsync(int eventId)
            => await _repository.GetCountByEventIdAsync(eventId);

        private async Task InvalidateEventsCacheAsync(int registrationId)
        {
            try
            {
                await _cache.RemoveByTagAsync(CacheKeys.TagEvents, CancellationToken.None);
                _logger.LogInformation(
                    "[EventRegistration] Registration {RegistrationId} persisted, invalidated cache tag {Tag}",
                    registrationId,
                    CacheKeys.TagEvents);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "[EventRegistration] Redis unavailable, cache invalidation skipped for {Tag}",
                    CacheKeys.TagEvents);
            }
        }

        private async Task SendConfirmationEmailAsync(
            RegisterForEventDto dto,
            string eventTitle,
            DateTime startDate,
            DateTime endDate,
            string location)
        {
            var startDateStr = startDate.ToString("dddd, MMMM d, yyyy");
            var startTimeStr = startDate.ToString("h:mm tt");
            var endTimeStr = endDate.ToString("h:mm tt");

            var safeFullName = WebUtility.HtmlEncode(dto.FullName ?? string.Empty);
            var safeEventTitle = WebUtility.HtmlEncode(eventTitle);
            var safeLocation = WebUtility.HtmlEncode(location);

            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                  <style>
                    body {{ font-family: Arial, sans-serif; background: #f9f9f9; margin: 0; padding: 20px; }}
                    .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 24px rgba(0,0,0,0.1); }}
                    .header {{ background: linear-gradient(135deg, #4c1d95, #7c3aed); padding: 40px 32px; text-align: center; }}
                    .header h1 {{ color: white; margin: 0 0 8px; font-size: 28px; }}
                    .header p {{ color: #ddd6fe; margin: 0; }}
                    .body {{ padding: 40px 32px; }}
                    .event-card {{ background: #f5f3ff; border-left: 4px solid #7c3aed; padding: 24px; border-radius: 8px; margin: 24px 0; }}
                    .event-card h2 {{ margin: 0 0 16px; color: #1a1a1a; font-size: 22px; }}
                    .detail {{ display: flex; align-items: center; gap: 10px; margin-bottom: 10px; color: #555; font-size: 15px; }}
                    .footer {{ background: #1a1a1a; padding: 24px 32px; text-align: center; color: #666; font-size: 12px; }}
                    .footer a {{ color: #a78bfa; text-decoration: none; }}
                  </style>
                </head>
                <body>
                  <div class='container'>
                    <div class='header'>
                      <h1>Global Flame</h1>
                      <p>Event Registration Confirmed</p>
                    </div>

                    <div class='body'>
                      <p style='font-size: 16px; color: #333;'>
                        Dear <strong>{safeFullName}</strong>,
                      </p>
                      <p style='color: #555; line-height: 1.6;'>
                        Your registration has been confirmed! We are excited to have
                        you join us. Here are your event details:
                      </p>

                      <div class='event-card'>
                        <h2>{safeEventTitle}</h2>
                        <div class='detail'>📅 <span><strong>{startDateStr}</strong></span></div>
                        <div class='detail'>🕐 <span>{startTimeStr} — {endTimeStr}</span></div>
                        <div class='detail'>📍 <span>{safeLocation}</span></div>
                      </div>

                      <p style='color: #555; line-height: 1.6;'>
                        Please keep this email as your registration confirmation.
                        We will send you a reminder closer to the event date.
                      </p>

                      <p style='color: #555; line-height: 1.6;'>
                        We look forward to seeing you there!
                      </p>

                      <p style='margin-top: 32px; color: #333;'>
                        God bless you,<br/>
                        <strong>Global Flame Team</strong>
                      </p>
                    </div>

                    <div class='footer'>
                      <p>© {DateTime.UtcNow.Year} Global Flame. All rights reserved.</p>
                      <p>
                        Zarmaganda, Diye, Off Rayfield Road, Jos, Plateau State, Nigeria
                      </p>
                    </div>
                  </div>
                </body>
                </html>";

            await _emailSender.SendEmailAsync(
                dto.Email,
                $"✅ Registration Confirmed — {eventTitle}",
                body
            );
        }
    }
}