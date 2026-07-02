using GlobalFlameMinistry.API.DTOs.BulkEmail;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class BulkEmailMapper
    {
        public static BulkEmailResponseDto ToBulkEmailResponseDto(
            this BulkEmailMessage message)
        {
            return new BulkEmailResponseDto
            {
                Id = message.Id,
                Subject = message.Subject,
                ImageUrl = message.ImageUrl,
                TargetGroup = message.TargetGroup,
                Status = message.Status,
                TotalRecipients = message.TotalRecipients,
                SuccessCount = message.SuccessCount,
                FailedCount = message.FailedCount,
                ScheduledAt = message.ScheduledAt,
                SentAt = message.SentAt,
                CreatedOn = message.CreatedOn,
                CreatedByName = message.CreatedByName,
                ErrorMessage = message.ErrorMessage,
            };
        }

        public static BulkEmailMessage ToModel(
            this SendBulkEmailDto dto, string htmlBody, int totalRecipients, string status, string? adminUserId, string? adminName)
        {
            return new BulkEmailMessage
            {
                Subject = dto.Subject,
                HtmlBody = htmlBody,
                TargetGroup = dto.TargetGroup,
                CustomEmailsJson = dto.TargetGroup == "Custom" ? dto.CustomEmails : null,
                Status = status,
                TotalRecipients = totalRecipients,
                ScheduledAt = dto.ScheduledAt.HasValue
                    ? dto.ScheduledAt.Value.ToUniversalTime() : null,
                ImageUrl = dto.ImageUrl,
                CreatedByUserId = adminUserId,
                CreatedByName = adminName,
                CreatedOn = DateTime.UtcNow,
            };
        }

        public static List<BulkEmailResponseDto> ToDtoList(
            this IEnumerable<BulkEmailMessage> messages)
        {
            return messages.Select(m => m.ToBulkEmailResponseDto()).ToList();
        }
    }
}