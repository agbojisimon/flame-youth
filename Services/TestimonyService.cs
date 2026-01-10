using g_flame_youth.DTOs.Testimony;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using g_flame_youth.Mappers;

namespace g_flame_youth.Services
{
    public class TestimonyService : ITestimonyService
    {
        private readonly ITestimonyRepository _testimonyRepo;
        public TestimonyService(ITestimonyRepository testimonyRepo)
        {
            _testimonyRepo = testimonyRepo;
        }

        public async Task<TestimonyResponseDto> CreateTestimonyAsync(CreateTestimonyDto createDto, string userId)
        {
            var testimony = createDto.ToTestimonyFromCreateDto();

            testimony.AppUserId = userId;
            testimony.CreatedAt = DateTime.UtcNow;

            await _testimonyRepo.CreateTestimonyAsync(testimony);

            return testimony.ToTestimonyResponseDto();
        }

        public Task<bool> DeleteTestimonyAsync(int Id)
        {
            return _testimonyRepo.DeleteTestimonyAsync(Id);
        }

        public async Task<List<TestimonyResponseDto>> GetTestimoniesAsync(TestimonyQueryObject query)
        {
            var testimonies = await _testimonyRepo.GetTestimoniesAsync(query);

            return testimonies.Select(t => t.ToTestimonyResponseDto()).ToList();
        }

        public async Task<TestimonyResponseDto?> GetTestimonyByIdAsync(int Id)
        {
            var testimony = await _testimonyRepo.GetTestimonyByIdAsync(Id);

            if (testimony == null)
                return null;

            return testimony.ToTestimonyResponseDto();
        }
    }
}