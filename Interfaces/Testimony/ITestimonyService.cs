using g_flame_youth.DTOs.Testimony;
using g_flame_youth.Helpers;
using g_flame_youth.Models;

namespace g_flame_youth.Interfaces
{
    public interface ITestimonyService
    {
        Task<List<TestimonyResponseDto>> GetTestimoniesAsync(TestimonyQueryObject query);
        Task<TestimonyResponseDto?> GetTestimonyByIdAsync(int Id);
        Task<TestimonyResponseDto> CreateTestimonyAsync(CreateTestimonyDto createDto, string userId);
        Task<bool> DeleteTestimonyAsync(int Id);
    }
}