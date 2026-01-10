using g_flame_youth.Data;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using g_flame_youth.Models;
using Microsoft.EntityFrameworkCore;

namespace g_flame_youth.Repository
{
    public class TestimonyRepository : ITestimonyRepository
    {
        private readonly AppDbContext _context;
        public TestimonyRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateTestimonyAsync(Testimony testimony)
        {
            await _context.Testimonies.AddAsync(testimony);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteTestimonyAsync(int Id)
        {
            var testimony = await _context.Testimonies.FindAsync(Id);

            if (testimony == null)
                return false;

            _context.Testimonies.Remove(testimony);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Testimony>> GetTestimoniesAsync(TestimonyQueryObject query)
        {
            var testimonies = _context.Testimonies.Include(t => t.User).AsQueryable();

            // Filter by user
            if (!string.IsNullOrWhiteSpace(query.AppUserId))
                testimonies = testimonies.Where(t => t.AppUserId == query.AppUserId);

            // Filter by approval status
            if (query.Status.HasValue)
                testimonies = testimonies.Where(t => t.Status == query.Status.Value);

            // Sorting (default: CreatedAt)
            testimonies = query.IsDescending
                ? testimonies.OrderByDescending(t => t.CreatedAt)
                : testimonies.OrderBy(t => t.CreatedAt);

            // Pagination
            testimonies = testimonies
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);

            return await _context.Testimonies.Include(t => t.User).OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public Task<Testimony?> GetTestimonyByIdAsync(int Id)
        {
            return _context.Testimonies.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == Id);
        }
    }
}