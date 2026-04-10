using GlobalFlameMinistry.API.DTOs.Ministry;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class MinistryMapper
    {
        public static MinistryResponseDto ToMinistryResponseDto(this MinistryDepartment ministry)
        {
            return new MinistryResponseDto
            {
                Id = ministry.Id,
                Name = ministry.Name,
                Slug = ministry.Slug,
                ShortDescription = ministry.ShortDescription,
                Description = ministry.Description,
                CoverImageUrl = ministry.CoverImageUrl,
                LeaderName = ministry.LeaderName,
                LeaderTitle = ministry.LeaderTitle,
                LeaderImageUrl = ministry.LeaderImageUrl,
                ContactEmail = ministry.ContactEmail,
                DisplayOrder = ministry.DisplayOrder,
                IsPublished = ministry.IsPublished,
                CreatedOn = ministry.CreatedOn,
                UpdatedOn = ministry.UpdatedOn,
            };
        }

        public static MinistryDepartment ToModel(this CreateMinistryDto createDto)
        {
            return new MinistryDepartment
            {
                Name = createDto.Name,
                Slug = GenerateSlug(createDto.Name),
                ShortDescription = createDto.ShortDescription,
                Description = createDto.Description,
                CoverImageUrl = createDto.CoverImageUrl,
                LeaderName = createDto.LeaderName,
                LeaderTitle = createDto.LeaderTitle,
                LeaderImageUrl = createDto.LeaderImageUrl,
                ContactEmail = createDto.ContactEmail,
                DisplayOrder = createDto.DisplayOrder,
                IsPublished = createDto.IsPublished,
                CreatedOn = DateTime.UtcNow,
            };
        }

        public static void ApplyUpdate(
            this MinistryDepartment ministry, UpdateMinistryDto updateDto)
        {
            ministry.Name = updateDto.Name;
            ministry.Slug = GenerateSlug(updateDto.Name);
            ministry.ShortDescription = updateDto.ShortDescription;
            ministry.Description = updateDto.Description;
            ministry.CoverImageUrl = updateDto.CoverImageUrl;
            ministry.LeaderName = updateDto.LeaderName;
            ministry.LeaderTitle = updateDto.LeaderTitle;
            ministry.LeaderImageUrl = updateDto.LeaderImageUrl;
            ministry.ContactEmail = updateDto.ContactEmail;
            ministry.DisplayOrder = updateDto.DisplayOrder;
            ministry.IsPublished = updateDto.IsPublished;
            ministry.UpdatedOn = DateTime.UtcNow;
        }

        public static List<MinistryResponseDto> ToDtoList(
            this IEnumerable<MinistryDepartment> ministries)
        {
            return ministries
                .Select(m => m.ToMinistryResponseDto())
                .ToList();
        }

        // SLUG GENERATOR 
        public static string GenerateSlug(string name)
        {
            return name
                .ToLowerInvariant()
                .Trim()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace("&", "and")
                .Replace(",", "")
                .Replace(".", "")
                .Replace("/", "-");
        }
    }
}