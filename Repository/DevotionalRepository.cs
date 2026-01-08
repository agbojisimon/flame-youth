using g_flame_youth.Data;
using g_flame_youth.Helpers.Queries;
using g_flame_youth.Interfaces;
using g_flame_youth.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace g_flame_youth.Repository
{
    public class DevotionalRepository : IDevotionalRepository
    {
        private readonly AppDbContext _context;
        public DevotionalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateDevotionalAsync(Devotional devotional)
        {
            try
            {
                await _context.Devotionals.AddAsync(devotional);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
            {
                throw new InvalidOperationException(
                    "A devotional for this date already exists."
                );
            }
        }

        public async Task<bool> DeleteDevotionalAsync(int Id)
        {
            var devotional = await _context.Devotionals.FirstOrDefaultAsync(d => d.Id == Id);

            if (devotional == null)
                return false;

            devotional.IsDeleted = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Devotional?> GetDevotionalByIdAsync(int Id)
        {
            return await _context.Devotionals.FirstOrDefaultAsync(d => d.Id == Id);
        }

        public async Task<List<Devotional>> GetDevotionalsAsync(DevotionalQueryObject query)
        {
            var devotionalQuery = _context.Devotionals.AsQueryable();

            if (query.IsPublished.HasValue)
                devotionalQuery = devotionalQuery.Where(d => d.IsPublished == query.IsPublished.Value);

            if (query.StartDate.HasValue)
                devotionalQuery = devotionalQuery.Where(d => d.DevotionalDate >= query.StartDate.Value);

            if (query.EndDate.HasValue)
                devotionalQuery = devotionalQuery.Where(d => d.DevotionalDate <= query.EndDate.Value);

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                devotionalQuery = query.SortBy switch
                {
                    "Title" => query.IsDescending
                        ? devotionalQuery.OrderByDescending(d => d.Title)
                        : devotionalQuery.OrderBy(d => d.Title),
                    "CreatedAt" => query.IsDescending
                        ? devotionalQuery.OrderByDescending(d => d.CreatedAt)
                        : devotionalQuery.OrderBy(d => d.CreatedAt),
                    _ => query.IsDescending
                        ? devotionalQuery.OrderByDescending(d => d.DevotionalDate)
                        : devotionalQuery.OrderBy(d => d.DevotionalDate)
                };
            }
            else
            {
                devotionalQuery = devotionalQuery.OrderByDescending(d => d.DevotionalDate);
            }
            devotionalQuery = devotionalQuery.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize);

            return await devotionalQuery.ToListAsync();
        }

        public async Task<Devotional?> GetTodayDevotionalAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await _context.Devotionals.Where(d => d.DevotionalDate == today && d.IsPublished).FirstOrDefaultAsync();
        }

        public async Task UpdateDevotionalAsync(Devotional devotional)
        {
            _context.Devotionals.Update(devotional);

            await _context.SaveChangesAsync();
        }
    }
}