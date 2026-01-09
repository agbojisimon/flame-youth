
using g_flame_youth.Helpers;
using g_flame_youth.Models;

namespace g_flame_youth.Interfaces
{
    public interface ITestimonyRepository
    {
        Task<List<Testimony>> GetTestimoniesAsync(TestimonyQueryObject query);
        Task<Testimony?> GetTestimonyByIdAsync(int Id);
        Task CreateTestimonyAsync(Testimony testimony);
        Task<bool> DeleteTestimonyAsync(int Id);
    }
}