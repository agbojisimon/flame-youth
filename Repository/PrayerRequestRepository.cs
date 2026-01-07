using g_flame_youth.Data;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using g_flame_youth.Models;
using Microsoft.EntityFrameworkCore;

namespace g_flame_youth.Repository
{
    public class PrayerRequestRepository : IPrayerRequestRepository
    {
        private readonly AppDbContext _context;
        public PrayerRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreatePrayerAsync(PrayerRequest request)
        {
            await _context.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var prayer = await _context.PrayerRequests.FirstOrDefaultAsync(p => p.id == id);

            if (prayer == null)
                return false;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PrayerRequest?> GetByIdAsync(int id)
        {
            return await _context.PrayerRequests.FirstOrDefaultAsync(p => p.id == id);
        }

        public async Task<List<PrayerRequest>> GetPrayerRequestsAsync(PrayerReqeustQueryObject query)
        {
            var prayers = _context.PrayerRequests.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Content))
            {
                prayers = prayers.Where(e => e.Content.Contains(query.Content));
            }
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                prayers = query.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase) ? (query.IsDescending ? prayers.OrderByDescending(p => p.CreatedAt) : prayers.OrderBy(p => p.CreatedAt)) : prayers;
            }
            else
            {
                prayers = prayers.OrderByDescending(p => p.CreatedAt);
            }

            var skip = (query.PageNumber - 1) * query.PageSize;

            return await prayers.Skip(skip).Take(query.PageSize).ToListAsync();
        }
    }
}