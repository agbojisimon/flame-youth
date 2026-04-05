using GlobalFlameMinistry.API.DTOs.Counselling;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class CounsellingMapper
    {
        public static CounsellingResponseDto ToResponseDto(this CounsellingRequest request)
        {
            return new CounsellingResponseDto
            {
                Id = request.Id,
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Topic = request.Topic,
                Message = request.Message,
                PreferredContact = request.PreferredContact,
                AssignedTo = request.AssignedTo,
                AssignedToEmail = request.AssignedToEmail,
                Status = request.Status.ToString(),
                AppUserId = request.AppUserId,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt
            };
        }

        public static CounsellingRequest ToModel(
            this CreateCounsellingRequestDto dto, string? appUserId)
        {
            return new CounsellingRequest
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Topic = dto.Topic,
                Message = dto.Message,
                PreferredContact = dto.PreferredContact,
                AppUserId = appUserId,
                Status = CounsellingStatus.New,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static List<CounsellingResponseDto> ToDtoList(
            this IEnumerable<CounsellingRequest> requests)
        {
            return requests.Select(r => r.ToResponseDto()).ToList();
        }
    }
}