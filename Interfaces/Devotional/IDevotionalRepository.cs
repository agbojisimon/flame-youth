using g_flame_youth.Helpers.Queries;
using g_flame_youth.Models;

namespace g_flame_youth.Interfaces
{
    public interface IDevotionalRepository
    {
        Task<Devotional?> GetTodayDevotionalAsync();
        Task<List<Devotional>> GetDevotionalsAsync(DevotionalQueryObject query);
        Task<Devotional?> GetDevotionalByIdAsync(int Id);
        Task CreateDevotionalAsync(Devotional devotional);
        Task UpdateDevotionalAsync(Devotional devotional);
        Task<bool> DeleteDevotionalAsync(int Id);
    }
}