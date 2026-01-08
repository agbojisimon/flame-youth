using g_flame_youth.DTOs.Devotional;
using g_flame_youth.Helpers.Queries;
using g_flame_youth.Interfaces;
using g_flame_youth.Mappers;

namespace g_flame_youth.Services
{
    public class DevotionalService : IDevotionalService
    {
        private readonly IDevotionalRepository _devoRepo;
        public DevotionalService(IDevotionalRepository devoRepo)
        {
            _devoRepo = devoRepo;
        }

        public async Task<DevotionalResponseDto> CreateDevotionalAsync(CreateDevotionalDto createDto)
        {
            var existing = await _devoRepo.GetDevotionalsAsync(new DevotionalQueryObject
            {
                StartDate = createDto.DevotionalDate,
                EndDate = createDto.DevotionalDate
            });

            if (existing.Any())
                throw new InvalidOperationException("A devotional for this date already exists.");

            var devotional = createDto.ToDevotionalFromCreateDto();

            await _devoRepo.CreateDevotionalAsync(devotional);

            return devotional.ToDevotionalResponseDto();
        }

        public async Task<bool> DeleteDevotionalAsync(int id)
        {
            var devotional = await _devoRepo.GetDevotionalByIdAsync(id);

            if (devotional == null)
                return false;

            return await _devoRepo.DeleteDevotionalAsync(id);
        }

        public async Task<DevotionalResponseDto?> GetDevotionalByIdAsync(int id)
        {
            var devotional = await _devoRepo.GetDevotionalByIdAsync(id);

            if (devotional == null)
                return null;

            return devotional.ToDevotionalResponseDto();
        }

        public async Task<List<DevotionalResponseDto>> GetDevotionalsAsync(DevotionalQueryObject query)
        {
            var devotionals = await _devoRepo.GetDevotionalsAsync(query);

            return devotionals.Select(d => d.ToDevotionalResponseDto()).ToList();
        }

        public async Task<DevotionalResponseDto?> GetTodayDevotionalAsync()
        {
            var nigeriaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");

            var nigeriaNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, nigeriaTimeZone);
            var today = DateOnly.FromDateTime(nigeriaNow);

            var entity = await _devoRepo.GetDevotionalsAsync(new DevotionalQueryObject
            {
                StartDate = today,
                EndDate = today,
                IsPublished = true
            });

            var devotional = entity.FirstOrDefault();

            if (devotional == null)
                return null;

            return devotional.ToDevotionalResponseDto();
        }

        public async Task<List<DevotionalResponseDto>> PreviewDevotionalsAsync(DevotionalQueryObject query)
        {
            var devotionals = await _devoRepo.GetDevotionalsAsync(query);

            return devotionals.Select(d => d.ToDevotionalResponseDto()).ToList();
        }

        public async Task<DevotionalResponseDto> UpdateDevotionalAsync(int id, UpdateDevotionalDto updateDto)
        {
            var devotional = await _devoRepo.GetDevotionalByIdAsync(id);
            if (devotional == null)
                throw new InvalidOperationException("Devotional not found.");

            var existing = await _devoRepo.GetDevotionalsAsync(new DevotionalQueryObject
            {
                StartDate = updateDto.DevotionalDate,
                EndDate = updateDto.DevotionalDate
            });

            if (existing.Any(d => d.Id != devotional.Id))
                throw new InvalidOperationException("Another devotional already exists for this date.");

            devotional.Title = updateDto.Title;
            devotional.Content = updateDto.Content;
            devotional.DevotionalDate = updateDto.DevotionalDate;
            devotional.UpdatedAt = DateTime.UtcNow;

            await _devoRepo.UpdateDevotionalAsync(devotional);

            return devotional.ToDevotionalResponseDto();
        }

        public async Task<DevotionalResponseDto> PublishDevotionalAsync(int id)
        {
            var devotional = await _devoRepo.GetDevotionalByIdAsync(id);

            if (devotional == null)
                throw new InvalidOperationException("Devotional not found.");

            if (devotional.IsPublished)
                throw new InvalidOperationException("Devotional is already published.");

            devotional.IsPublished = true;
            devotional.UpdatedAt = DateTime.UtcNow;

            await _devoRepo.UpdateDevotionalAsync(devotional);

            return devotional.ToDevotionalResponseDto();
        }

        public async Task<List<DevotionalResponseDto>> GetPublishedDevotionalsAsync(DevotionalQueryObject query)
        {
            var nigeriaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, nigeriaTimeZone));

            query.IsPublished = true;
            query.EndDate = today;

            var devotionals = await _devoRepo.GetDevotionalsAsync(query);

            return devotionals.Select(d => d.ToDevotionalResponseDto()).ToList();
        }
    }
}